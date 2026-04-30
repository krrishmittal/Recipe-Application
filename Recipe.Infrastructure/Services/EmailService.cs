using System.Net;
using System.Net.Mail;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Infrastructure.Services;

/// <summary>
/// Sends application emails using configured email settings and templates.
/// </summary>
public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    private SmtpClient CreateSmtpClient()
    {
        var settings = config.GetSection("EmailSettings");
        return new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(settings["SenderEmail"], settings["Password"]),
            EnableSsl = true
        };
    }

    private static string LoadTemplate(string templateName)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Handlers", "EmailTemplates", templateName);
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Sends a welcome email to a newly registered user.
    /// </summary>
    public async Task SendWelcomeEmailAsync(string toEmail, string name)
    {
        try
        {
            var settings = config.GetSection("EmailSettings");
            var body = LoadTemplate("WelcomeTemplate.html")
                            .Replace("{{NAME}}", name)
                            .Replace("{{CURRENT_YEAR}}", DateTime.Now.Year.ToString());

            var mail = new MailMessage
            {
                From = new MailAddress(settings["SenderEmail"]!, settings["SenderName"]),
                Subject = "Welcome to Recipe App!",
                IsBodyHtml = true,
                Body = body
            };

            mail.To.Add(toEmail);
            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(mail);

            logger.LogInformation("Welcome email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
            throw;
        }
    }

    /// <summary>
    /// Sends an OTP email for password recovery.
    /// </summary>
    public async Task SendOtpEmailAsync(string toEmail, string name, string otpCode)
    {
        try
        {
            var settings = config.GetSection("EmailSettings");
            var body = LoadTemplate("OtpTemplate.html")
                            .Replace("{{NAME}}", name)
                            .Replace("{{OTP_CODE}}", otpCode)
                            .Replace("{{EXPIRY_MINUTES}}", config["OtpSettings:ExpiryMinutes"])
                            .Replace("{{CURRENT_YEAR}}", DateTime.Now.Year.ToString());

            var mail = new MailMessage
            {
                From = new MailAddress(settings["SenderEmail"]!, settings["SenderName"]),
                Subject = "Your Password Reset OTP",
                IsBodyHtml = true,
                Body = body
            };

            mail.To.Add(toEmail);
            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(mail);

            logger.LogInformation("OTP email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
            throw;
        }
    }
}
