using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelWaveFinal.DB;
using HotelWaveFinal.Models;

namespace HotelWaveFinal.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PaymentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.payments.Include(p => p.User).Include(p => p.Booking);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.payments
                .Include(p => p.User)
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.applicationUsers, "Id", "Id");
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId");
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PaymentMethod,Amount,PaymentDate,PaymentStatus,BookingId,AccountNumber,ExpirationDate,CVV,UserId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.applicationUsers, "Id", "Id", payment.UserId);
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.applicationUsers, "Id", "Id", payment.UserId);
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,PaymentMethod,Amount,PaymentDate,PaymentStatus,BookingId,AccountNumber,ExpirationDate,CVV,UserId")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["UserId"] = new SelectList(_context.applicationUsers, "Id", "Id", payment.UserId);
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", payment.BookingId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.payments
                .Include(p => p.User)
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.payments.FindAsync(id);
            if (payment != null)
            {
                _context.payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.payments.Any(e => e.PaymentId == id);
        }

        // POST: Payments/ProcessPayment
        //[HttpPost]
        //public async Task<IActionResult> ProcessPayment(Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var _payment = new Payment
        //        {
        //            UserId = payment.UserId,
        //            BookingId = payment.BookingId,
        //            Amount = payment.Amount,
        //            PaymentMethod = payment.PaymentMethod,
        //            PaymentDate = DateOnly.FromDateTime(DateTime.Now),
        //            PaymentStatus = "Payment Completed",
        //            AccountNumber = payment.AccountNumber,
        //            ExpirationDate = payment.ExpirationDate,
        //            CVV = payment.CVV
        //        };
        //        var _booking = await _context.Bookings
        //            .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);

        //        if (_booking != null)
        //        {
        //            _booking.Status = "Payment Completed";
        //            _context.Bookings.Update(_booking);
        //        }

        //        _context.payments.Add(_payment);
        //        await _context.SaveChangesAsync();

        //        int paymentId = _payment.PaymentId;

        //        // Send confirmation or notification to customer and admin
        //        return RedirectToAction("PaymentConfirmation", new { paymentId = paymentId });
        //    }

        //    return View(payment);
        //}

        // GET: Payments/PaymentConfirmation
        public IActionResult PaymentConfirmation(int paymentId)
        {
            var payment = _context.payments.FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }


        [HttpGet]
        public IActionResult CreatePayment(int bookingId, string userId, double amount)
        {
            // Retrieve the booking and user details from the database
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            //var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            //if (booking == null || userId == null)
            //{
            //    // Handle cases where booking or user does not exist
            //    return NotFound("Booking or User not found.");
            //}

            var payment = new Payment
            {
                BookingId = bookingId,
                Booking = booking, // Assign the booking navigation property
                UserId = userId,
                Amount = amount,
                PaymentDate = DateOnly.FromDateTime(DateTime.Now), // Set the current date as the default
                PaymentStatus = "Pending", // Default status, can be updated later
                PaymentMethod = "Credit Card" // You can initialize a default method or leave it for the view to update
            };

            // Pass the payment object to the CreatePayment view
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Create a new payment object with the submitted data
                var newPayment = new Payment
                {
                    UserId = payment.UserId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount, // The total amount from the booking
                    PaymentMethod = payment.PaymentMethod,
                    ExpirationDate = payment.ExpirationDate,
                    CVV = payment.CVV,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentStatus = "Payment Completed" // Update status upon successful payment
                };

                // Optionally, retrieve the booking associated with this payment
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);

                // Add the new payment to the database
                await _context.payments.AddAsync(newPayment);
                await _context.SaveChangesAsync();

                // Get the payment ID for confirmation
                int paymentId = newPayment.PaymentId;

                // Send confirmation or notification to customer and admin if needed
                // Redirect to a confirmation page
                return RedirectToAction("PaymentConfirmation", new { paymentId = paymentId });
            }

            // If the model state is not valid, return the view with the current payment object
            return View(payment);
        }


    }
}
