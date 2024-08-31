using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public double TotalCost { get; set; }
        public DateOnly DayCreated { get; set; }
        public status Status { get; set; }

        public enum status
        {
            Pending,
            Confirmed,
            Canceled
        }

        public int UserId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("UserId")]
        public User User { get; set; }  //This is a Navigation Property.

        public int RoomId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("RoomId")]
        public Room Room { get; set; }  //This is a Navigation Property.

        public int ServiceId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }   // This is a Navigation Property.
    }
}
