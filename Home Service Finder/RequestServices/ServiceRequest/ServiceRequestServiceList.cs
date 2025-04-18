using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.ServicesList;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Table("ServiceRequestServiceList", Schema = "Requests")]
    public class ServiceRequestServiceList
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("ServiceRequest")]
        public Guid RequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        [ForeignKey("ServiceList")]
        public Guid ServiceListId { get; set; }
        public ServiceList ServiceList { get; set; }
    }
}
