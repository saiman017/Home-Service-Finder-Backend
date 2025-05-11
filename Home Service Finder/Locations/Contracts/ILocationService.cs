using Home_Service_Finder.Locations.Dtos;

namespace Home_Service_Finder.Locations.Contracts
{
    public interface ILocationService
    {

        Task<APIResponse> GetLocationAsync(Guid id);
        Task<APIResponse> SaveLocationAsync( LocationRequestDto locationRequestDto);
        Task<APIResponse> UpdateLocationAsync(Guid userId, LocationRequestDto locationRequestDto);

    }
}
