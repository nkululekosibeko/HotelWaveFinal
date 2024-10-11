using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelWaveFinal.DB;
using HotelWaveFinal.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelWaveFinal.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public BookingsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Bookings
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Bookings.Include(b => b.Room);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        [Authorize]
        public IActionResult Create()
        {
            // Fetch only available rooms
            var availableRooms = _context.Rooms
                .Where(r => r.IsAvailable) // Filter rooms where IsAvailable is true
                .Select(r => new { r.RoomId, r.RoomNumber })
                .ToList();

            // Populate the ViewData for the Room dropdown
            ViewData["RoomId"] = new SelectList(availableRooms, "RoomId", "RoomNumber"); // Display RoomNumber in dropdown
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CustomerName,PhoneNumber,CheckIn,CheckOut,NumberOfAdults,NumberOfChildren,RoomId,Status")] Booking booking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (booking.CheckOut <= booking.CheckIn)
            {
                ModelState.AddModelError("CheckOut", "Check-out date must be later than check-in date.");
            }

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("UserId", "User ID could not be retrieved. Please log in again.");
                return View(booking);
            }

            var roomBooked = _context.Bookings
                .Where(b => b.RoomId == booking.RoomId &&
                            (b.CheckIn < booking.CheckOut && b.CheckOut > booking.CheckIn))
                .Any();

            // Validate number of adults and children against room capacity
            //if (booking.NumberOfAdults > room.MaxAdults)
            //{
            //    ModelState.AddModelError("NumberOfAdults", $"The selected room can only accommodate up to {room.MaxAdults} adults.");
            //}

            //if (booking.NumberOfChildren > room.MaxChildren)
            //{
            //    ModelState.AddModelError("NumberOfChildren", $"The selected room can only accommodate up to {room.MaxChildren} children.");

            //    if (roomBooked)
            //{
            //    ModelState.AddModelError("RoomId", "The selected room is already booked for the selected dates.");
            //}



            if (!ModelState.IsValid)
            {
                var room = await _context.Rooms.FindAsync(booking.RoomId);
                if (room != null)
                {
                    double adultRate = 1.0;
                    double childRate = 0.5;

                    booking.TotalCost = room.PricePerNight * (booking.NumberOfAdults * adultRate + booking.NumberOfChildren * childRate);

                    // Mark the room as unavailable
                    room.IsAvailable = false;
                    _context.Update(room);
                }

                booking.UserId = userId;

                if (!User.IsInRole("Admin"))
                {
                    booking.Status = "Pending";
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["success"] = "Booking Created Successfully";
                return RedirectToAction("Details", new { id = booking.BookingId });
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", booking.RoomId);
            return View(booking);
        }




        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

            // Get all bookings made by the current user
            var userBookings = _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId); // Filter by the logged-in user

            return View(await userBookings.ToListAsync());
        }



        // GET: Bookings/Edit/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Populate the dropdown for Status
            ViewBag.StatusList = new SelectList(new List<string> { "Pending", "Confirmed", "Checked-In", "Checked-Out", "Cancelled" }, booking.Status);

            ViewData["UserId"] = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", booking.UserId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,CustomerName,PhoneNumber,CheckIn,CheckOut,NumberOfAdults,NumberOfChildren,RoomId,Status")] Booking updatedBooking)
        {
            if (id != updatedBooking.BookingId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var existingBooking = await _context.Bookings.FindAsync(id);
                    if (existingBooking == null)
                    {
                        return NotFound();
                    }

                    // Preserve the original UserId
                    updatedBooking.UserId = existingBooking.UserId;

                    _context.Entry(existingBooking).CurrentValues.SetValues(updatedBooking);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Booking updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(updatedBooking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.StatusList = new SelectList(new List<string> { "Pending", "Confirmed", "Checked-In", "Checked-Out", "Cancelled"}, updatedBooking.Status);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", updatedBooking.RoomId);
            ViewData["UserId"] = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", updatedBooking.UserId);
            return View(updatedBooking);
        }




        // GET: Bookings/Delete/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Booking deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
