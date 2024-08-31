using System.ComponentModel.DataAnnotations;

namespace HotelWaveFinal.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public double BasePrice { get; set; }
    }
}
