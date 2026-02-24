namespace Server.Services.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndStoreOtpAsync(int userId);
    Task<bool> ValidateOtpAsync(int userId, string otpCode);
}