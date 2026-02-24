using System.Net;
using System.Net.Mail;
using Server.Services.Interfaces;

namespace Server.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    private SmtpClient CreateSmtpClient()
    {
        var settings = _config.GetSection("EmailSettings");
        return new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(settings["SenderEmail"], settings["Password"]),
            EnableSsl = true
        };
    }

    private string LoadTemplate(string templateName)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Handlers", "EmailTemplates", templateName);
        return File.ReadAllText(path);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string name)
    {
        try
        {
            var settings = _config.GetSection("EmailSettings");
            var body = LoadTemplate("WelcomeTemplate.html")
                            .Replace("{{NAME}}", name);

            var mail = new MailMessage
            {
                From = new MailAddress(settings["SenderEmail"]!, settings["SenderName"]),
                Subject = "Welcome to Recipe App! 🍽️",
                IsBodyHtml = true,
                Body = body
            };

            mail.To.Add(toEmail);
            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(mail);

            _logger.LogInformation("Welcome email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendOtpEmailAsync(string toEmail, string name, string otpCode)
    {
        try
        {
            var settings = _config.GetSection("EmailSettings");
            var body = LoadTemplate("OtpTemplate.html")
                            .Replace("{{NAME}}", name)
                            .Replace("{{OTP_CODE}}", otpCode)
                            .Replace("{{EXPIRY_MINUTES}}", _config["OtpSettings:ExpiryMinutes"]);

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

            _logger.LogInformation("OTP email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
            throw;
        }
    }
}