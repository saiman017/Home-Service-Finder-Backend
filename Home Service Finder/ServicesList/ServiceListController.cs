using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.Roles.Dtos;
using Home_Service_Finder.ServicesList.Contracts;
using Home_Service_Finder.ServicesList.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.ServicesList
{
    [Route("api/serviceList")]
    [ApiController]
    public class ServiceListController : ControllerBase
    {

        private readonly IServiceListService _serviceListService;

        public ServiceListController(IServiceListService serviceListService)
        {
            _serviceListService = serviceListService;
        }

        [HttpPost]
        public async Task<APIResponse> AddServiceListAsync([FromBody] ServiceListRequestDto serviceListRequestDto)
        {
            var apiResponse = await _serviceListService.AddServiceList(serviceListRequestDto);
            return apiResponse;
        }

        [HttpGet]
        public async Task<APIResponse> GetAllServiceListAsync()
        {
            var apiResponse = await _serviceListService.GetAllServiceList();
            return apiResponse;
        }


        [HttpGet("{id}")]
        public async Task<APIResponse> GetServiceListByIdAsync(Guid id)
        {
            var apiResponse = await _serviceListService.GetServiceListById(id);
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteServiceListAsync(Guid id)
        {
            var apiResponse = await _serviceListService.DeleteServiceList(id);
            return apiResponse;
        }

        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateServiceListAsync(Guid id, [FromBody] ServiceListRequestDto serviceListRequestDto)
        {
            var apiResponse = await _serviceListService.UpdateServiceList(id, serviceListRequestDto);
            return apiResponse;
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<APIResponse> GetServiceListByCategoryAsync(Guid categoryId)
        {
            var apiResponse = await _serviceListService.GetServiceListByCategoryId(categoryId);
            return apiResponse;
        }
    }
}
