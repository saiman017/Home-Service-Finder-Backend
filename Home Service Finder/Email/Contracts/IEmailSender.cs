using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Home_Service_Finder.Email.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);
        Task SendOTPEmailAsync(string email, string otpCode, int validityMinutes);
    }

    
    
}