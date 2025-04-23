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
        public string ImagePath { get; set; } // e.g., /uploads/serviceRequestImages/image1.jpg
    }

}
