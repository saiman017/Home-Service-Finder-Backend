using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Table("ServiceRequestImage", Schema = "Requests")]
    public class ServiceRequestImage
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("ServiceRequest")]
        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }


        [Column("ImagePath", TypeName = "TEXT")]
        public string ImagePath { get; set; }
    }
}
