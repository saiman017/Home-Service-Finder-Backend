using System.Threading.Tasks;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.Dtos;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.ServiceProvider.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.Users.ServiceProvider
{
    [Route("api/serviceProvider")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviderService _serviceProviderService;

        public ServiceProviderController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        [HttpGet]
        //[Authorize(Roles = "admin")]
        public async Task<APIResponse> GetAllServiceProviderAsync()
        {
            var apiResponse = await _serviceProviderService.GetAllServiceProviderAsync();
            return apiResponse;
        }


        [HttpGet("{id}")]
        public async Task<APIResponse> GetServiceProviderById(Guid id)
        {
            var apiResponse = await _serviceProviderService.GetServiceProviderById(id);
            return apiResponse;
        }

        [HttpPost]
        public async Task<APIResponse> AddServiceProviderAsync([FromBody] ServiceProviderRequestDto requestDto)
        {
            var apiResponse = await _serviceProviderService.AddServiceProviderAsync(requestDto);
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteServiceProviderAsync(Guid id)
        {
            var apiResponse = await _serviceProviderService.DeleteServiceProviderAsync(id);
            return apiResponse;

        }

        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, [FromBody] ServiceProviderUpdateRequestDto requestDto)
        {
            var apiResponse = await _serviceProviderService.UpdateServiceProvider(id, requestDto);
            return apiResponse;
        }

        [HttpGet("statistics/{providerId}")]
        public async Task<APIResponse> GetServiceProviderStatisticsAsync(Guid providerId)
        {
            var apiResponse = await _serviceProviderService.GetServiceProviderStatisticsAsync(providerId);
            return apiResponse;

        }
    }
}

