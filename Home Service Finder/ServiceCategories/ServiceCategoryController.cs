//using Home_Service_Finder.ServiceCategories.Contracts;
//using Home_Service_Finder.ServiceCategories.Dtos;
//using Home_Service_Finder.Users.Dtos;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Home_Service_Finder.ServiceCategories
//{
//    [Route("api/serviceCategory")]
//    [ApiController]
//    public class ServiceCategoryController : ControllerBase
//    {
//        private readonly IServiceCategoryService _serviceCategoryService;

//        public ServiceCategoryController(IServiceCategoryService serviceCategoryService)
//        {
//            _serviceCategoryService = serviceCategoryService;

//        }

//        [HttpGet]
//        public async Task<APIResponse> GetAllServiceCategoryAsync()
//        {
//            var apiResponse = await _serviceCategoryService.GetAllServiceCategoryAsync();
//            return apiResponse;
//        }

//        [HttpGet("{id}")]
//        public async Task<APIResponse> GetServiceCategoyById(Guid id)
//        {
//            var apiResponse = await _serviceCategoryService.GetByIdServiceCategoryAsync(id);
//            return apiResponse;
//        }

//        [HttpPost]
//        public async Task<APIResponse> AddServiceCategoryAsync([FromBody] ServiceCategoryRequestDto serviceCategoryRequestDto)
//        {
//            var apiResponse = await _serviceCategoryService.AddServiceCategoryAsync(serviceCategoryRequestDto);
//            return apiResponse;
//        }

//        [HttpDelete("{id}")]
//        public async Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
//        {
//            var apiResponse = await _serviceCategoryService.DeleteServiceCategoryAsync(id);
//            return apiResponse;
//        }

//        [HttpPut("{id}")]
//        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id,[FromBody]ServiceCategoryRequestDto serviceCategoryRequestDto )
//        {
//            var apiResponse = await _serviceCategoryService.UpdateServiceCategoryAsync(id, serviceCategoryRequestDto);
//            return apiResponse;
//        }

//        [HttpPost("uploadImage/{categoryId}")]
//        public async Task<APIResponse> UploadCategoryImage(Guid categoryId, IFormFile file)
//        {
//            var result = await _serviceCategoryService.UploadCategoryImageAsync(categoryId, file);
//            return result;
//        }

//    }
//}

// ServiceCategoryController.cs
using System;
using System.Threading.Tasks;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServiceCategories.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.ServiceCategories
{
    [Route("api/serviceCategory")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategoryService _service;

        public ServiceCategoryController(IServiceCategoryService serviceCategoryService)
            => _service = serviceCategoryService;

        // GET /api/serviceCategory
        [HttpGet]
        public Task<APIResponse> GetAllServiceCategoryAsync()
            => _service.GetAllServiceCategoryAsync();

        // GET /api/serviceCategory/{id}
        [HttpGet("{id:guid}")]
        public Task<APIResponse> GetServiceCategoryById(Guid id)
            => _service.GetByIdServiceCategoryAsync(id);

        // POST /api/serviceCategory
        // Bind from multipart/form-data
        [HttpPost]
        public Task<APIResponse> AddServiceCategoryAsync([FromForm] ServiceCategoryRequestDto dto)
            => _service.AddServiceCategoryAsync(dto);

        // PUT /api/serviceCategory/{id}
        // Also multipart/form-data
        [HttpPut("{id:guid}")]
        public Task<APIResponse> UpdateServiceCategoryAsync(
            Guid id,
            [FromForm] ServiceCategoryRequestDto dto)
            => _service.UpdateServiceCategoryAsync(id, dto);

        // DELETE /api/serviceCategory/{id}
        [HttpDelete("{id:guid}")]
        public Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
            => _service.DeleteServiceCategoryAsync(id);
    }
}
