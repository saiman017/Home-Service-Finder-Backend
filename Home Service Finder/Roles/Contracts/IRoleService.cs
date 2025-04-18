using Home_Service_Finder.Roles.Dtos;

namespace Home_Service_Finder.Roles.Contracts
{
    public interface IRoleService 
    {
        Task<APIResponse> AddRoleAsync(RoleRequestDto roleRequestDto);
        Task<APIResponse> GetAllRoleAsync();
        Task<APIResponse> GetRoleByIDAsync(Guid id);
        Task<APIResponse> DeleteRoleAysnc(Guid id);
        Task<APIResponse> UpdateRoleAysnc(Guid id, RoleRequestDto roleRequestDto);

        Task<APIResponse> GetRoleByNameAsync(string name);

        

    }
}
