using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWaveFinal.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public double TotalCost { get; set; }

        public int ServiceTypeId { get; set; }  // Here i am Creating Foreign Keys.
        [ForeignKey("ServiceTypeId")]
        [ValidateNever]
        public ServiceType ServiceType { get; set; }  //This is a Navigation Property
    }
}
