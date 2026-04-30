using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the forgot password command.
/// </summary>
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ForgotPasswordCommandHandler class.
    /// </summary>
    public ForgotPasswordCommandHandler(
        RecipeDbContext db,
        IOtpService otpService,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _db = db;
        _otpService = otpService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Forgot password request for {Email}", request.Request.Email);

            var email = request.Request.Email.ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is not null)
            {
                var otpCode = await _otpService.GenerateAndStoreOtpAsync(user.Id);
                await _emailService.SendOtpEmailAsync(user.Email, user.Name, otpCode);
            }

            return ApiResponse<bool>.Ok(true, "If the email exists, an OTP has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(ForgotPasswordCommandHandler));
            return ApiResponse<bool>.Fail("Password recovery failed due to an unexpected error.", 500, nameof(ForgotPasswordCommand));
        }
    }
}
