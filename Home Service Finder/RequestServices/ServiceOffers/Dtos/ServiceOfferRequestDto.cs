namespace Home_Service_Finder.RequestServices.ServiceOffers.Dtos
{
    public class ServiceOfferRequestDto
    {
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public decimal OfferedPrice { get; set; }
    }
}
