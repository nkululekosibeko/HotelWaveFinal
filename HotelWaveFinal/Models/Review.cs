using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public DateOnly DayCreated { get; set; }

        public int UserId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("UserId")]
        public User User { get; set; }  //This is a Navigation Property.
    }
}
