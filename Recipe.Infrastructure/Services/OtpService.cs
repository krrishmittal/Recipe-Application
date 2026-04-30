using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Infrastructure.Services;

/// <summary>
/// Generates, stores, and validates one-time password codes.
/// </summary>
public class OtpService : IOtpService
{
    private readonly RecipeDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<OtpService> _logger;

    /// <summary>
    /// Initializes a new instance of the OtpService class.
    /// </summary>
    public OtpService(RecipeDbContext db, IConfiguration config, ILogger<OtpService> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Generates an OTP code, stores it, and returns the code.
    /// </summary>
    public async Task<string> GenerateAndStoreOtpAsync(Guid userId)
    {
        var otpCode = Random.Shared.Next(100000, 999999).ToString();
        var expiry = int.Parse(_config["OtpSettings:ExpiryMinutes"]!);
        var hash = Handlers.PasswordHashing.HashP(otpCode);

        // Check if row already exists for this user
        var existing = await _db.OtpRecords.FirstOrDefaultAsync(o => o.UserId == userId);

        if (existing is not null)
        {
            _logger.LogDebug("Updating existing OTP for userId: {UserId}", userId);
            existing.OtpCode = hash;
            existing.ExpiresAt = DateTime.UtcNow.AddMinutes(expiry);
            existing.IsUsed = false;
            existing.CreatedAt = DateTime.UtcNow;
            _db.OtpRecords.Update(existing);
        }
        else
        {
            _logger.LogDebug("Creating new OTP entry for userId: {UserId}", userId);
            var record = new OtpRecord
            {
                UserId = userId,
                OtpCode = hash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiry),
                IsUsed = false,
                CreatedAt=DateTime.UtcNow
            };
            await _db.OtpRecords.AddAsync(record);
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("OTP generated for userId: {UserId}", userId);
        return otpCode;
    }

    /// <summary>
    /// Validates an OTP code for a user.
    /// </summary>
    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        var otp = await _db.OtpRecords.FirstOrDefaultAsync(o =>
            o.UserId == userId &&
            !o.IsUsed &&
            o.ExpiresAt > DateTime.UtcNow);

        if (otp is null)
        {
            _logger.LogWarning("No valid OTP found for userId: {UserId}", userId);
            return false;
        }

        if (!Handlers.PasswordHashing.VerifyP(otpCode, otp.OtpCode))
        {
            _logger.LogWarning("Invalid OTP for userId: {UserId}", userId);
            return false;
        }

        otp.IsUsed = true;
        await _db.SaveChangesAsync();

        _logger.LogInformation("OTP validated successfully for userId: {UserId}", userId);
        return true;
    }
}
