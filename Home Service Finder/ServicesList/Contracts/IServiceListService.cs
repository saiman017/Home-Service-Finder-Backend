using Home_Service_Finder.ServicesList.Dtos;

namespace Home_Service_Finder.ServicesList.Contracts
{
    public interface IServiceListService
    {
        Task<APIResponse> AddServiceList(ServiceListRequestDto serviceListRequestDto);
        Task<APIResponse> GetAllServiceList();

        Task<APIResponse> GetServiceListById(Guid id);

        Task<APIResponse> DeleteServiceList(Guid id);

        Task<APIResponse> UpdateServiceList(Guid id, ServiceListRequestDto serviceListRequestDto);
        Task<APIResponse> GetServiceListByCategoryId(Guid categoryId);



    }
        
}
