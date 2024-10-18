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



        [HttpGet]
        public IActionResult CreatePayment(int bookingId, string userId, double amount)
        {
            // Retrieve the booking and user details from the database
            //var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            //var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            //if (booking == null || userId == null)
            //{
            //    // Handle cases where booking or user does not exist
            //    return NotFound("Booking or User not found.");
            //}

            var payment = new Payment
            {
                BookingId = bookingId,
                UserId = userId,
                Amount = amount,
            };

            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payment payment)
        {
            if (ModelState.IsValid)
            {
                var newPayment = new Payment
                {
                    UserId = payment.UserId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount, 
                    PaymentMethod = payment.PaymentMethod,
                    AccountNumber = payment.AccountNumber,
                    ExpirationDate = payment.ExpirationDate,
                    CVV = payment.CVV,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentStatus = "Payment Completed"
                };

                var _booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);

                if (_booking != null)
                {
                    _booking.Status = "Payment Completed";
                    _context.Bookings.Update(_booking);
                }

                // Add the new payment to the database
                _context.payments.Add(newPayment);
                await _context.SaveChangesAsync();

                // Get the payment ID of the newly created payment for confirmation
                int paymentId = newPayment.PaymentId;

                // Redirect to the PaymentConfirmation action with the payment ID
                return RedirectToAction("PaymentConfirmation", new { paymentId = paymentId });
            }

            // If the model state is invalid, return the same view with errors
            return View(payment);
        }

        [HttpGet]
        public IActionResult PaymentConfirmation(int paymentId)
        {
            var payment = _context.payments.FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

    }
}
