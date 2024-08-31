using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public double Amount { get; set; }
        public DateOnly PaymentDate {  get; set; }

        public int UserId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("UserId")]
        public User User { get; set; }  //This is a Navigation Property.
    }
}
