using System.ComponentModel.DataAnnotations;

namespace Home_Service_Finder.Email.Dtos
{
    public class EmailOTPVerificationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OTPCode { get; set; }
    }
}
