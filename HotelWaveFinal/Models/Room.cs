using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }

        public bool IsAvailable { get; set; }
        public double PricePerNight { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        [ValidateNever]

        public List<Booking>? Bookings { get; set; }

        public int RoomTypeId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("RoomTypeId")]
        [ValidateNever]
        public RoomType RoomType { get; set; }  //This is a Navigation Property.
    }
}
