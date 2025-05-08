using System.ComponentModel.DataAnnotations;

namespace Home_Service_Finder.RequestServices.ServiceRequest.Dtos
{
    public class CancelRequestDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }
    }
}
