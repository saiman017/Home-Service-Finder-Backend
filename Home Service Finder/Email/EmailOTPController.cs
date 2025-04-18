using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Email.Dtos;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.Email
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private readonly IEmailOTPService _emailOTPService;

        public OTPController(IEmailOTPService emailOTPService)
        {
            _emailOTPService = emailOTPService;
        }

        [HttpPost("verify")]
        public async Task<APIResponse> VerifyOTP([FromBody] EmailOTPVerificationDto verificationDto)
        {
            var apiResponse = await _emailOTPService.VerifyOTP(verificationDto);
            return apiResponse;
        }

        [HttpPost("resend")]
        public async Task<APIResponse> ResendOTP([FromBody] EmailDto emailDto)
        {
            var apiResponse = await _emailOTPService.ResendOTP(emailDto.Email);
            return apiResponse;
        }
    }

    public class EmailDto
    {
        public string Email { get; set; }
    }
}