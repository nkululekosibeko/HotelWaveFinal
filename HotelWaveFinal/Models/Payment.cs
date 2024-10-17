using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public double Amount { get; set; }

        [Display(Name = "Payment Date")]
        public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required(ErrorMessage = "Payment status is required.")]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Booking Id")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        [ValidateNever]
        public Booking Booking { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Account number must be exactly 16 digits.")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        [RegularExpression(@"\d{2}/\d{2}", ErrorMessage = "Expiration date must be in MM/YY format.")]
        [Display(Name = "Expiration Date")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be exactly 3 digits.")]
        [Display(Name = "CVV Number")]
        public string CVV { get; set; }

        [Display(Name = "Customer Id")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
