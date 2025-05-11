using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
using Home_Service_Finder.RequestServices.ServiceRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Route("api/serviceRequest")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IHubContext<ServiceRequestHub> _hubContext;

        public ServiceRequestController(
            IServiceRequestService serviceRequestService,
            IHubContext<ServiceRequestHub> hubContext)
        {
            _serviceRequestService = serviceRequestService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<APIResponse> CreateServiceRequest([FromBody] ServiceRequestRequestDto serviceRequestRequestDto)
        {
            var apiResponse = await _serviceRequestService.CreateServiceRequestAsync(serviceRequestRequestDto);

        
            if (apiResponse.Success && apiResponse.Data != null)
            {
                if (apiResponse.Data is Guid requestId)
                {
                    var requestResponse = await _serviceRequestService.GetServiceRequestByIdAsync(requestId);
                    if (requestResponse.Success && requestResponse != null)
                    {
                        // Notify all providers subscribed to this category
                        await _hubContext.Clients.Group($"Category_{serviceRequestRequestDto.ServiceCategoryId}")
                            .SendAsync("NewRequestCreated", requestResponse);
                    }
                }
            }

            return apiResponse;
        }

        [HttpGet]
        public async Task<APIResponse> GetAllServiceRequests()
        {
            var apiResponse = await _serviceRequestService.GetAllServiceRequestAsync();
            return apiResponse;
        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetServiceRequestById(Guid id)
        {
            var apiResponse = await _serviceRequestService.GetServiceRequestByIdAsync(id);
            return apiResponse;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<APIResponse> GetRequestsByCustomerId(Guid customerId)
        {
            var apiResponse = await _serviceRequestService.GetRequestByCustomerId(customerId);
            return apiResponse;
        }

        [HttpGet("customer/{customerId}/active")]
        public async Task<APIResponse> GetActiveRequestsByCustomerId(Guid customerId)
        {
            var apiResponse = await _serviceRequestService.GetActiveRequestByCustomerId(customerId);
            return apiResponse;
        }

        [HttpGet("customer/{customerId}/pending")]
        public async Task<APIResponse> GetPendingRequestByCustomerId(Guid customerId)
        {
            var apiResponse = await _serviceRequestService.GetPendingRequestByCustomerId(customerId);
            return apiResponse;
        }

        [HttpGet("{categoryId}/pending")]
        public async Task<APIResponse> GetPendingRequestsByCategory(Guid categoryId)
        {
            var apiResponse = await _serviceRequestService.GetPendingRequestByCategory(categoryId);
            return apiResponse;
        }

        [HttpGet("category/{categoryId}")]
        public async Task<APIResponse> GetRequestsByServiceCategory(Guid categoryId)
        {
            var apiResponse = await _serviceRequestService.GetRequestByServiceCategory(categoryId);
            return apiResponse;
        }

        [HttpPut("{requestId}/status")]
        public async Task<APIResponse> UpdateServiceRequestStatus(Guid requestId, [FromBody] string status)
        {
            var apiResponse = await _serviceRequestService.UpdateServiceRequestStatusAsync(requestId, status);

            // If status was updated successfully, notify subscribers
            if (apiResponse.Success && apiResponse.Data != null)
            {
                var requestDetails = await _serviceRequestService.GetServiceRequestByIdAsync(requestId);
                if (requestDetails.Success && requestDetails.Data != null)
                {
                    var request = requestDetails.Data as ServiceRequestResponseDto;
                    if (request != null)
                    {
                        // Notify category subscribers about the status change
                        await _hubContext.Clients.Group($"Category_{request.ServiceCategoryId}")
                            .SendAsync("RequestStatusUpdated", request);

                        // Also notify the customer
                        await _hubContext.Clients.Group($"User_{request.CustomerId}")
                            .SendAsync("YourRequestStatusUpdated", request);
                    }
                }
            }

            return apiResponse;


        }



        [HttpPut("{requestId}/cancel")]
        public async Task<IActionResult> CancelServiceRequest(Guid requestId, [FromBody] Guid customerId)
        {
            var request = await _serviceRequestService.GetServiceRequestByIdAsync(requestId);
            if (!request.Success || request.Data == null)
            {
                return NotFound("Request not found");
            }

            var requestData = request.Data as ServiceRequestResponseDto;
            if (requestData.CustomerId != customerId)
            {
                return Unauthorized("You can only cancel your own requests");
            }

            if (requestData.Status == "Completed" || requestData.Status == "Cancelled")
            {
                return BadRequest($"Cannot cancel a request that is {requestData.Status}");
            }

            var result = await _serviceRequestService.CancelServiceRequestAsync(
                requestId, customerId);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            // Get provider ID if request was accepted
            Guid? providerId = null;
            if (requestData.Status == "Accepted")
            {
                // You'll need to implement this based on your data model
                // providerId = await GetAssignedProviderId(requestId);
            }

            // Use the new unified notification
            await _hubContext.Clients.Group($"Category_{requestData.ServiceCategoryId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    CategoryId = requestData.ServiceCategoryId,
                    Status = "Cancelled",
                    CancelledAt = DateTime.UtcNow
                });

            await _hubContext.Clients.Group($"Customer_{customerId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    Status = "Cancelled",
                    CancelledAt = DateTime.UtcNow,
                    Message = "Your service request has been cancelled"
                });

            if (providerId.HasValue)
            {
                await _hubContext.Clients.Group($"Provider_{providerId}")
                    .SendAsync("RequestCancelled", new
                    {
                        RequestId = requestId,
                        Status = "Cancelled",
                        CancelledAt = DateTime.UtcNow,
                        Message = "An assigned service request has been cancelled"
                    });
            }

            return Ok(new
            {
                Success = true,
                Message = "Request cancelled successfully"
            });
        }



        [HttpDelete("customer/{customerId}")]
        public async Task<APIResponse> DeleteRequestsByCustomerId(Guid customerId)
        {
            var apiResponse = await _serviceRequestService.DeleteRequestByCustomerId(customerId);
            return apiResponse;
        }

        [HttpPost("{requestId}/uploadImages")]
        public async Task<APIResponse> UploadImages(Guid requestId, List<IFormFile> files)
        {
            var apiResponse = await _serviceRequestService.UploadServiceRequestImagesAsync(requestId, files);
            return apiResponse;
        }

        [HttpPut("{requestId}/cancelAccepted")]
        public async Task<APIResponse> CancelAccepted(
       Guid requestId,
       [FromBody] CancelRequestDto dto)
        {
           
            var apiResponse = await _serviceRequestService.CancelAcceptedRequestAsync(requestId, dto);
            
            return apiResponse;
        }

    }
}