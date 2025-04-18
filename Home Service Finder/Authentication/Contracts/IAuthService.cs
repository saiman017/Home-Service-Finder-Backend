using Home_Service_Finder.Authentication.Dtos;

namespace Home_Service_Finder.Authentication.Contracts
{
    public interface IAuthService
    {

        Task<APIResponse> Login(LoginRequestDto requestDto);
        Task<APIResponse> LoginWithRefreshToken(string refreshToken);


    }
}
