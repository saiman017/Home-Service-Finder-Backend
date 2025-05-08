using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.RequestServices.ServiceRequest;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Ratings
{
    [Table("Rating", Schema = "Service")]
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        

        [ForeignKey("ServiceProvider")]
        
        public Guid ServiceProviderId { get; set; }
       



        [ForeignKey("ServiceRequest")]
        [Column("ServiceRequestId", TypeName = "uuid")]
        public Guid? ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }

        [Required]
        [Range(1, 5)]
        [Column("Value", TypeName = "int")]
        public int Value { get; set; }

        [Column("Comments", TypeName = "VARCHAR(500)")]
        public string? Comments { get; set; }

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
