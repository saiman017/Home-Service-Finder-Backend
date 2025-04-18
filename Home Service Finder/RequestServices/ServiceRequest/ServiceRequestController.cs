using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    [Route("api/serviceRequest")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpPost]
        public async Task<APIResponse> CreateServiceRequest([FromBody] ServiceRequestRequestDto serviceRequestRequestDto)
        {
            var apiResponse = await _serviceRequestService.CreateServiceRequestAsync(serviceRequestRequestDto);
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

        [HttpGet("category/{categoryId}")]
        public async Task<APIResponse> GetRequestsByServiceCategory(Guid categoryId)
        {
            var apiResponse = await _serviceRequestService.GetRequestByServiceCategory(categoryId);
            return apiResponse;
        }

        [HttpDelete("customer/{customerId}")]
        public async Task<APIResponse> DeleteRequestsByCustomerId(Guid customerId)
        {
            var apiResponse = await _serviceRequestService.DeleteRequestByCustomerId(customerId);
            return apiResponse;
        }
    }
}