namespace Home_Service_Finder.RequestServices.ServiceOffers.Dtos
{
    public class ServiceOfferResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string ProviderName { get; set; }
        public decimal OfferedPrice { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; } // "Pending", "Accepted", "Rejected", "Expired"
    }

    
}
