namespace Home_Service_Finder.RequestServices.ServiceOffers.Dtos
{
    public class StatusUpdateDto
    {
        public string Status { get; set; }
        public Guid RequestId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
