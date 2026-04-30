namespace Recipe.Application.Services.Interfaces;

/// <summary>
/// Defines email delivery operations used by the application.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an OTP email for password recovery.
    /// </summary>
    Task SendOtpEmailAsync(string toEmail, string name, string otpCode);
    /// <summary>
    /// Sends a welcome email to a newly registered user.
    /// </summary>
    Task SendWelcomeEmailAsync(string toEmail, string name);
}
