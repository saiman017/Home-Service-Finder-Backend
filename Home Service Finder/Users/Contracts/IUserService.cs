using Home_Service_Finder.Authentication.Dtos;
using Home_Service_Finder.Users.Dtos;

namespace Home_Service_Finder.Users.Contracts
{
    public interface IUserService
    {
        Task<APIResponse> GetAllUsers();
        Task<APIResponse> GetUserById(Guid id);
        Task<APIResponse> DeleteUser(Guid id);
        Task<APIResponse> AddUser(UserRequestDto userRequestDto);
        Task<APIResponse> UpdateUser(Guid id,UserUpdateRequestDto userUpdateRequestDto);
        Task<APIResponse> UploadProfilePicture(Guid userId, IFormFile file);
        Task<APIResponse> DeleteProfilePicture(Guid userId);



    }
}
