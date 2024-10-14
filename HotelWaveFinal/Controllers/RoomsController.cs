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

namespace HotelWaveFinal.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public RoomsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Rooms.Include(r => r.RoomType);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        [Authorize(Roles = SD.Role_Admin)]
        // GET: Rooms/Create
        public IActionResult Create()
        {
            // Populating the ViewBag for RoomTypeId dropdown
            ViewBag.RoomTypeId = new SelectList(_context.RoomTypes.ToList(), "RoomTypeId", "TypeName");
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,RoomNumber,IsAvailable,PricePerNight,RoomTypeId")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                TempData["success"] = "Room created Successfully";
                return RedirectToAction(nameof(Index));
            }

            // Repopulate the dropdown in case of validation errors
            ViewBag.RoomTypeId = new SelectList(_context.RoomTypes.ToList(), "RoomTypeId", "TypeName", room.RoomTypeId);
            return View(room);
        }






        // GET: Rooms/Edit/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["RoomTypeId"] = new SelectList(_context.RoomTypes, "RoomTypeId", "RoomTypeId", room.RoomTypeId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,RoomNumber,IsAvailable,PricePerNight,RoomTypeId")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Room updated Successfully";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
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
            ViewData["RoomTypeId"] = new SelectList(_context.RoomTypes, "RoomTypeId", "RoomTypeId", room.RoomTypeId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Room Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }


        public List<Room> CheckRoomAvailability(DateOnly checkInDate, DateOnly checkOutDate, int adults, int children)
        {
            // Query available rooms
            var availableRooms = _context.Rooms
                .Where(r => r.IsAvailable)
                .Where(r => r.MaxAdults >= adults && r.MaxChildren >= children)
                .Where(r => !r.Bookings.Any(b => b.CheckIn < checkOutDate && b.CheckOut > checkInDate)) // No conflicting bookings
                .ToList();

            return availableRooms;
        }

        [HttpPost]
        public IActionResult CheckAvailability(DateOnly checkInDate, DateOnly checkOutDate, int adults, int children)
        {
            var availableRooms = CheckRoomAvailability(checkInDate, checkOutDate, adults, children);

            if (availableRooms.Any())
            {
                // Pass available rooms to the view
                return View("AvailableRooms", availableRooms);
            }
            else
            {
                ViewBag.Message = "No rooms available for the selected criteria.";
                return View("NoAvailability");
            }
        }


        public IActionResult AvailableRoomsByType(int roomTypeId)
        {
            // Fetch rooms that are available and match the selected room type
            var rooms = _context.Rooms
                .Include(r => r.RoomType) // Include RoomType for display purposes
                .Where(r => r.RoomTypeId == roomTypeId && r.IsAvailable) // Filter by type and availability
                .ToList();

            // Pass the rooms list to the view
            return View(rooms);
        }
    }
}
