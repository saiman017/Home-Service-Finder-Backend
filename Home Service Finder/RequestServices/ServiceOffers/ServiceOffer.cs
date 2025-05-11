using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users.ServiceProvider;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    [Table("ServiceOffer", Schema = "Requests")]
    public class ServiceOffer
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("ServiceRequest")]
        public Guid ServiceRequestId { get; set; }
        public ServiceRequest.ServiceRequest ServiceRequest { get; set; }

        [ForeignKey("ServiceProvider")]
        public Guid ServiceProviderId { get; set; }
        public virtual Users.ServiceProvider.ServiceProvider ServiceProvider { get; set; }

        [Column("OfferedPrice", TypeName = "DECIMAL")]
        public decimal OfferedPrice { get; set; }

        [Column("SentAt", TypeName = "TIMESTAMPTZ")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Column("ExpiresAt", TypeName = "TIMESTAMPTZ")]
        public DateTime ExpiresAt { get; set; }
        [Column("Status", TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "Pending";

        [Column("PaymentStatus", TypeName = "BOOLEAN")]
        public bool PaymentStatus { get; set; } = false;

        [Column("PaymentReason", TypeName = "TEXT")] 
        public string? PaymentReason { get; set; } = null;
    }


  
    
}
