using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Email.Dtos;
using static System.Net.WebRequestMethods;

namespace Home_Service_Finder.Email
{
    public class EmailOTPService : IEmailOTPService
    {
        private readonly IUnitOfWork _db;
        private readonly IEmailSender _emailSender;
        private const int OTP_VALIDITY_MINUTES = 10;

        public EmailOTPService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _db = unitOfWork;
            _emailSender = emailSender;

        }
        public async Task<APIResponse> GenerateOTP(Guid userId)
        {
            var user = await _db.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            string otpCode = GenerateRandomOTP(); // otpCode code genrate

            EmailOTP newOTP = new EmailOTP
            {
                UserId = userId,
                Code = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(OTP_VALIDITY_MINUTES),
                IsUsed = false
            };

            await _db.Emails.AddAsync(newOTP);
            await _db.SaveChangesAsync();

            // Send OTP via email
            await _emailSender.SendOTPEmailAsync(user.Email, otpCode, OTP_VALIDITY_MINUTES);

            return ResponseHandler.GetSuccessResponse(newOTP,
                 $"OTP sent to {user.Email}. Valid for {OTP_VALIDITY_MINUTES} minutes.");
        }

        

        public async Task<APIResponse> ResendOTP(string email)
        {
            var user = await _db.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            if (user.IsEmailVerified)
            {
                return ResponseHandler.GetBadRequestResponse("Email already verified");
            }

            // Generate new OTP
            return await GenerateOTP(user.Id);
        }

        public async Task<APIResponse> VerifyOTP(EmailOTPVerificationDto verificationDto)
        {
            var user = await _db.Users.GetByEmailAsync(verificationDto.Email);
            if (user == null)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            // Check if OTP exists and is valid
            var otp = await _db.Emails.GetByCodeAndUserIdAsync(verificationDto.OTPCode, user.Id);
            if (otp == null)
            {
                return ResponseHandler.GetBadRequestResponse("Invalid or expired OTP");
            }

            // Mark OTP as used
            otp.IsUsed = true;
            _db.Emails.UpdateAsync(otp);

            // Mark user's email as verified
            user.IsEmailVerified = true;
            _db.Users.UpdateAsync(user);

            await _db.SaveChangesAsync();

            var response = new EmailOTPResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                IsVerified = true
            };

            return ResponseHandler.GetSuccessResponse(response, "Email verified successfully");
        }

        private string GenerateRandomOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

    }
}
