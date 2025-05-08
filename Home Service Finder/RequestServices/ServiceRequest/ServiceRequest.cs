using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Locations;
using Home_Service_Finder.Users.Users;
using Home_Service_Finder.ServiceCategories;
using Home_Service_Finder.RequestServices.ServiceOffers;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Table("ServiceRequest", Schema = "Requests")]
    public class ServiceRequest
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public User Customer { get; set; }

        [ForeignKey("Location")]
        public Guid LocationId { get; set; }
        public Location Location { get; set; }

        [ForeignKey("ServiceCategory")]
        public Guid ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; }

        [Column("Description", TypeName = "TEXT")]
        public string Description { get; set; }

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("ExpiresAt", TypeName = "TIMESTAMPTZ")]
        public DateTime ExpiresAt { get; set; }

        [Column("Status", TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected, Expired

        [Column("CancelReason", TypeName = "TEXT")]
        public string? CancelReason { get; set; }

        public ICollection<ServiceRequestServiceList> ServiceRequestServiceLists { get; set; }
        public ICollection<ServiceOffer> Serviceoffers { get; set; }

        // Snapshot of location details at request time
        [Column("LocationAddress", TypeName = "TEXT")]
        public string LocationAddress { get; set; }

        [Column("LocationCity", TypeName = "VARCHAR(100)")]
        public string LocationCity { get; set; }

        [Column("LocationPostalCode", TypeName = "VARCHAR(20)")]
        public string LocationPostalCode { get; set; }

        [Column("LocationLatitude", TypeName = "DOUBLE PRECISION")]
        public double LocationLatitude { get; set; }

        [Column("LocationLongitude", TypeName = "DOUBLE PRECISION")]
        public double LocationLongitude { get; set; }

        public ICollection<ServiceRequestImage> Images { get; set; }

    }
}
