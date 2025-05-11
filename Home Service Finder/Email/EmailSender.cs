using System.Net.Mail;
using System.Net;
using Home_Service_Finder.Email.Contracts;

namespace Home_Service_Finder.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var mailSettings = _configuration.GetSection("MailSettings");

            using var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(mailSettings["SenderEmail"], mailSettings["SenderName"]);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(mailSettings["Server"])
            {
                Port = int.Parse(mailSettings["Port"]),
                Credentials = new NetworkCredential(
                    mailSettings["Username"],  
                    mailSettings["Password"]),
                EnableSsl = bool.Parse(mailSettings["UseSSL"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false 
            };

            await client.SendMailAsync(message);
        }

        public async Task SendOTPEmailAsync(string email, string otpCode, int validityMinutes)
        {
            string subject = "Verify Your Email - Home Service Finder";
            string body = GetOTPEmailTemplate(email, otpCode, validityMinutes);

            await SendEmailAsync(email, subject, body);
        }

        private string GetOTPEmailTemplate(string email, string otpCode, int validityMinutes)
        {
            string appName = "Home Service Finder";
            string appUrl = "https://homeservicefinder.com";
            string supportEmail = "support@homeservicefinder.com";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Email Verification</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding: 20px 0;
            border-bottom: 1px solid #eee;
        }}
        .logo {{
            max-width: 150px;
            height: auto;
        }}
        .content {{
            padding: 30px 20px;
            text-align: center;
        }}
        .otp-code {{
            font-size: 32px;
            font-weight: bold;
            letter-spacing: 5px;
            color: #4a6ee0;
            margin: 20px 0;
            padding: 10px;
            background-color: #f0f4ff;
            border-radius: 5px;
            display: inline-block;
        }}
        .message {{
            margin-bottom: 30px;
            font-size: 16px;
        }}
        .note {{
            font-size: 14px;
            color: #666;
            margin-top: 30px;
            padding: 15px;
            background-color: #f9f9f9;
            border-radius: 5px;
        }}
        .validity {{
            font-weight: bold;
            color: #e74c3c;
        }}
        .footer {{
            text-align: center;
            padding: 20px;
            font-size: 12px;
            color: #999;
            border-top: 1px solid #eee;
        }}
        .social-links {{
            margin-top: 15px;
        }}
        .social-links a {{
            display: inline-block;
            margin: 0 10px;
            color: #4a6ee0;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{appName}</h1>
        </div>
        <div class='content'>
            <h2>Email Verification</h2>
            <p class='message'>Hi there,<br>Thank you for signing up with {appName}! To complete your registration, please use the following verification code:</p>
            <div class='otp-code'>{otpCode}</div>
            <p>Please enter this code on the verification page to confirm your email address.</p>
            <p class='validity'>This code is valid for <strong>{validityMinutes} minutes</strong> only.</p>
            <div class='note'>
                <p>If you didn't request this code, you can safely ignore this email. Someone might have entered your email address by mistake.</p>
            </div>
        </div>
        <div class='footer'>
            <p>Need help? Contact our support team at <a href='mailto:{supportEmail}'>{supportEmail}</a></p>
            <p>&copy; {DateTime.Now.Year} {appName}. All rights reserved.</p>
            <div class='social-links'>
                <a href='{appUrl}/terms'>Terms of Service</a> | 
                <a href='{appUrl}/privacy'>Privacy Policy</a>
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
    }
