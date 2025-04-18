using Home_Service_Finder.ServiceCategories.Dtos;

namespace Home_Service_Finder.ServiceCategories.Contracts
{
    public interface IServiceCategoryService
    {
        Task<APIResponse> GetAllServiceCategoryAsync();
        Task<APIResponse> GetByIdServiceCategoryAsync(Guid id);
        Task<APIResponse> DeleteServiceCategoryAsync(Guid id);
        Task<APIResponse> UpdateServiceCategoryAsync(Guid id, ServiceCategoryRequestDto serviceCategoryRequestDto);
        Task<APIResponse> AddServiceCategoryAsync(ServiceCategoryRequestDto serviceCategoryRequestDto);

    }
}
