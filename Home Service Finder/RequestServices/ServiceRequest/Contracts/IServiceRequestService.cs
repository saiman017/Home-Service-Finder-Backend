using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
using Home_Service_Finder.Roles.Dtos;

namespace Home_Service_Finder.RequestServices.ServiceRequest.Contracts
{
    public interface IServiceRequestService
    {
        Task<APIResponse> CreateServiceRequestAsync(ServiceRequestRequestDto serviceRequestRequestDto);

        Task<APIResponse> GetAllServiceRequestAsync();

        Task<APIResponse> GetServiceRequestByIdAsync(Guid id);
        Task<APIResponse> GetRequestByCustomerId(Guid customerId);

        Task<APIResponse> GetRequestByServiceCategory(Guid categoryId);

        Task<APIResponse> DeleteRequestByCustomerId(Guid customerId);






    }
}
