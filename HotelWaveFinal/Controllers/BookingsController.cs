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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,Name,PhoneNumber,CheckIn,CheckOut,NumberOfAdults,NumberOfChildren,RoomId,Status")] Booking booking)
        {
            // Get the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Ensure the user ID is retrieved correctly
            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case where the user ID is not found
                ModelState.AddModelError("UserId", "User ID could not be retrieved. Please log in again.");
                return View(booking);
            }

            booking.UserId = userId;
           

            if (booking.CheckOut <= booking.CheckIn)
            {
                ModelState.AddModelError("CheckOut", "Check-out date must be later than check-in date.");
            }

            // Validate if the selected room is available for the given dates
            var roomBooked = _context.Bookings
                .Where(b => b.RoomId == booking.RoomId &&
                            (b.CheckIn < booking.CheckOut && b.CheckOut > booking.CheckIn))
                .Any();

            if (roomBooked)
            {
                ModelState.AddModelError("RoomId", "The selected room is already booked for the selected dates.");
            }

            if (!ModelState.IsValid)
            {
                if (!User.IsInRole("Admin"))
                {
                    booking.Status = "Pending";
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["success"] = "Booking Created Successfully";
                return RedirectToAction("Details", new { id = booking.BookingId });
            }

            // Reload the RoomId select list if the model state is invalid
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,Name,PhoneNumber,CheckIn,CheckOut,NumberOfAdults,NumberOfChildren,RoomId,Status")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Booking updated Successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", booking.RoomId);
            return View(booking);
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
