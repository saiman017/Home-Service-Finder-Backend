namespace Home_Service_Finder.RequestServices.ServiceRequest.Dtos
{
    public class ServiceRequestRequestDto
    {
        public Guid CustomerId { get; set; }
        public Guid LocationId { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public string Description { get; set; }
        public List<Guid> ServiceListIds { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
