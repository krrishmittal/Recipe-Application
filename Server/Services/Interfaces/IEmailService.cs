namespace Server.Services.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string name, string otpCode);
    Task SendWelcomeEmailAsync(string toEmail, string name);
}