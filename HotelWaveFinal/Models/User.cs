using System.ComponentModel.DataAnnotations;

namespace HotelWaveFinal.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}
