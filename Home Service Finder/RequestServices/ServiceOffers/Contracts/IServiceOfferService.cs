using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;

namespace Home_Service_Finder.RequestServices.ServiceOffers.Contracts
{
    public interface IServiceOfferService
    {
        Task<APIResponse> CreateServiceOfferAsync(ServiceOfferRequestDto serviceOfferRequestDto);
        Task<APIResponse> GetOffersByRequestIdAsync(Guid requestId);
        Task<APIResponse> AcceptOfferAsync(Guid offerId, Guid customerId);
        Task<APIResponse> GetOffersByProviderIdAsync(Guid providerId);
        Task<APIResponse> RejectOfferAsync(Guid offerId, Guid customerId);

        Task<APIResponse> GetOfferByIdAsync(Guid offerId);

        //Task<APIResponse> UpdateOfferStatusAsync(Guid offerId, string status);
        Task<APIResponse> UpdateOfferStatusAsync(Guid offerId, string status, Guid requestId, Guid customerId);


        Task<APIResponse> UpdatePaymentStatusAsync(Guid offerId, bool paymentStatus);

        Task<APIResponse> UpdatePaymentReasonAsync(Guid offerId, string paymentReason);
    }
}
