using System.ComponentModel.DataAnnotations;

namespace HotelWaveFinal.Models
{
    public class ServiceType
    {
        [Key]
        public int ServiceTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
    }
}
