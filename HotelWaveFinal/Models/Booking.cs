using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int RoomId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("RoomId")]
        [ValidateNever]
        public Room Room { get; set; }  //This is a Navigation Property.

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }
        public string Status { get; set; } = "Pending";
        public double TotalCost { get; set; }

    }
}
