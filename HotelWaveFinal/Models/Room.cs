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

        public int RoomTypeId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("RoomTypeId")]
        public RoomType RoomType { get; set; }  //This is a Navigation Property.
    }
}
