using System.Reflection.Metadata.Ecma335;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    [Route("api/serviceOffer")]
    [ApiController]
    public class ServiceOfferController : ControllerBase
    {
        private readonly IServiceOfferService _serviceOfferService;

        public ServiceOfferController(IServiceOfferService serviceOfferService)
        {
            _serviceOfferService = serviceOfferService;
        }

        [HttpPost]
        public async Task<APIResponse> CreateServiceOffer([FromBody] ServiceOfferRequestDto serviceOfferRequestDto)
        {
            return await _serviceOfferService.CreateServiceOfferAsync(serviceOfferRequestDto);
        }

        [HttpGet("request/{requestId}")]
        public async Task<APIResponse> GetOffersByRequestId(Guid requestId)
        {
            return await _serviceOfferService.GetOffersByRequestIdAsync(requestId);
        }

        [HttpGet("provider/{providerId}")]
        public async Task<APIResponse> GetOffersByProviderId(Guid providerId)
        {
            return await _serviceOfferService.GetOffersByProviderIdAsync(providerId);
        }

        [HttpPut("{offerId}/accept")]
        public async Task<APIResponse> AcceptOffer(Guid offerId, [FromBody] Guid customerId)
        {
            return await _serviceOfferService.AcceptOfferAsync(offerId, customerId);
        }

        [HttpPut("{offerId}/reject")]
        public async Task<APIResponse> RejectOffer(Guid offerId, [FromBody] Guid customerId)
        {
            return await _serviceOfferService.RejectOfferAsync(offerId, customerId);
        }

        [HttpPut("{offerId}/status")]
        public async Task<APIResponse> UpdateOfferStatus(Guid offerId, [FromBody] StatusUpdateDto statusUpdate)
        {
            return await _serviceOfferService.UpdateOfferStatusAsync(offerId, statusUpdate.Status, statusUpdate.RequestId, statusUpdate.CustomerId);
        }



        [HttpGet("offer/{offerId}")]
        public async Task<APIResponse> GetOfferByIdAsync(Guid offerId)
        {
            return await _serviceOfferService.GetOfferByIdAsync(offerId);
        }

        [HttpPut("{offerId}/payment")]
        public async Task<APIResponse> UpdatePaymentStatus(
           Guid offerId,
           [FromBody] PaymentUpdateDto dto)
        {
            var apiResponse = await _serviceOfferService.UpdatePaymentStatusAsync(offerId, dto.PaymentStatus);

            return apiResponse;

        }

        [HttpPut("{offerId}/reason")]
        public async Task<APIResponse> UpdatePaymentReason(Guid offerId, [FromBody] PaymentReasonDto dto)
        {
            var apiResponse = await _serviceOfferService.UpdatePaymentReasonAsync(offerId, dto.PaymentReason);
            return apiResponse;
            
        }
    }


}