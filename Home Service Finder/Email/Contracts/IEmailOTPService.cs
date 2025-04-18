using Home_Service_Finder.Email.Dtos;

namespace Home_Service_Finder.Email.Contracts
{
    public interface IEmailOTPService
    {
        Task<APIResponse> GenerateOTP(Guid userId);
        Task<APIResponse> VerifyOTP(EmailOTPVerificationDto verificationDto);
        Task<APIResponse> ResendOTP(string email);
    }
}
