namespace Recipe.Application.Services.Interfaces;

/// <summary>
/// Defines one-time password generation and validation operations.
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates an OTP code, stores it, and returns the code.
    /// </summary>
    Task<string> GenerateAndStoreOtpAsync(Guid userId);
    /// <summary>
    /// Validates an OTP code for a user.
    /// </summary>
    Task<bool> ValidateOtpAsync(Guid userId, string otpCode);
}
