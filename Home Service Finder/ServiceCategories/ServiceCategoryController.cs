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

        [HttpGet]
        public Task<APIResponse> GetAllServiceCategoryAsync()
            => _service.GetAllServiceCategoryAsync();

        [HttpGet("{id:guid}")]
        public Task<APIResponse> GetServiceCategoryById(Guid id)
            => _service.GetByIdServiceCategoryAsync(id);

        [HttpPost]
        public Task<APIResponse> AddServiceCategoryAsync([FromForm] ServiceCategoryRequestDto dto)
            => _service.AddServiceCategoryAsync(dto);

        [HttpPut("{id:guid}")]
        public Task<APIResponse> UpdateServiceCategoryAsync(
            Guid id,
            [FromForm] ServiceCategoryRequestDto dto)
            => _service.UpdateServiceCategoryAsync(id, dto);

        [HttpDelete("{id:guid}")]
        public Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
            => _service.DeleteServiceCategoryAsync(id);
    }
}
