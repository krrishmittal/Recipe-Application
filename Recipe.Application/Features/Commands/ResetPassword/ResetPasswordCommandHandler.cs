using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the reset password command.
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly IOtpService _otpService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ResetPasswordCommandHandler class.
    /// </summary>
    public ResetPasswordCommandHandler(
        RecipeDbContext db,
        IOtpService otpService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _db = db;
        _otpService = otpService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Reset password attempt for {Email}", request.Request.Email);

            var email = request.Request.Email.ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user is null)
            {
                return ApiResponse<bool>.Fail("Invalid or expired OTP.", 400, nameof(ResetPasswordCommand));
            }

            var isValid = await _otpService.ValidateOtpAsync(user.Id, request.Request.OtpCode);
            if (!isValid)
            {
                return ApiResponse<bool>.Fail("Invalid or expired OTP.", 400, nameof(ResetPasswordCommand));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password reset successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(ResetPasswordCommandHandler));
            return ApiResponse<bool>.Fail("Password reset failed due to an unexpected error.", 500, nameof(ResetPasswordCommand));
        }
    }
}
