using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Authentication.Contracts
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);

    }
}
