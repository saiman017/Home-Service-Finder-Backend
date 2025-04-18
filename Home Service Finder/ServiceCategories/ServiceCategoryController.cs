using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServiceCategories.Dtos;
using Home_Service_Finder.Users.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.ServiceCategories
{
    [Route("api/serviceCategory")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategoryService _serviceCategoryService;

        public ServiceCategoryController(IServiceCategoryService serviceCategoryService)
        {
            _serviceCategoryService = serviceCategoryService;
            
        }

        [HttpGet]
        public async Task<APIResponse> GetAllServiceCategoryAsync()
        {
            var apiResponse = await _serviceCategoryService.GetAllServiceCategoryAsync();
            return apiResponse;
        }

        [HttpGet("{id}")]
        public async Task<APIResponse> GetServiceCategoyById(Guid id)
        {
            var apiResponse = await _serviceCategoryService.GetByIdServiceCategoryAsync(id);
            return apiResponse;
        }

        [HttpPost]
        public async Task<APIResponse> AddServiceCategoryAsync([FromBody] ServiceCategoryRequestDto serviceCategoryRequestDto)
        {
            var apiResponse = await _serviceCategoryService.AddServiceCategoryAsync(serviceCategoryRequestDto);
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public async Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
        {
            var apiResponse = await _serviceCategoryService.DeleteServiceCategoryAsync(id);
            return apiResponse;
        }

        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id,[FromBody]ServiceCategoryRequestDto serviceCategoryRequestDto )
        {
            var apiResponse = await _serviceCategoryService.UpdateServiceCategoryAsync(id, serviceCategoryRequestDto);
            return apiResponse;
        }
    }
}
