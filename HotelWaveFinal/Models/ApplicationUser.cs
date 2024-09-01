using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelWaveFinal.Models
{
    public class ApplicationUser : IdentityUser 
    {
        [Required]
        public int Name {  get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? LoyaltyPoints { get; set; }

    }
}
