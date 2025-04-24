namespace Home_Service_Finder.RequestServices.ServiceRequest.Dtos
{
    public class ServiceRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }

        public Guid LocationId { get; set; }

        public Guid ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public string Status { get; set; }

        public List<Guid> ServiceListIds { get; set; }
        public List<string> ServiceListNames { get; set; }

        public string LocationAddress { get; set; }
        public string LocationCity { get; set; }
        public string LocationPostalCode { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        public List<string> ServiceRequestImagePaths { get; set; } = new List<string>();

    }
}
