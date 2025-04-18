using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users.ServiceProvider;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Table("ServiceOffer", Schema = "Requests")]
    public class ServiceOffer
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("ServiceRequest")]
        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        [ForeignKey("ServiceProvider")]
        public Guid ServiceProviderId { get; set; }
        public virtual Users.ServiceProvider.ServiceProvider ServiceProvider { get; set; }

        [Column("OfferedPrice", TypeName = "DECIMAL")]
        public decimal OfferedPrice { get; set; }

        [Column("Message", TypeName = "VARCHAR(500)")]
        public string? Message { get; set; }

        [Column("SentAt", TypeName = "TIMESTAMPTZ")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Column("IsAccepted", TypeName = "BOOLEAN")]
        public bool IsAccepted { get; set; } = false;
    }
}
