namespace Home_Service_Finder.Authentication.Dtos
{
    public class RefreshTokenResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
