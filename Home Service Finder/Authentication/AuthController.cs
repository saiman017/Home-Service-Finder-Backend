using Home_Service_Finder.Authentication.Contracts;
using Home_Service_Finder.Authentication.Dtos;
using Home_Service_Finder.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<APIResponse>  Login([FromBody]LoginRequestDto loginRequestDto)
        {
            var apiResponse = await _authService.Login(loginRequestDto);
            return apiResponse;
        }

        [HttpPost("refresh")]
        public async Task<APIResponse> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var apiResponse = await _authService.LoginWithRefreshToken(refreshTokenRequestDto.RefreshToken);
            return apiResponse;
        }
    }
}
