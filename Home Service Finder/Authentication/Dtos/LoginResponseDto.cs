using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Authentication.Dtos
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }
        public string?  RefreshToken { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public Guid UserId { get; set; }
        public bool IsEmailVerified { get; set; }

    }
}
