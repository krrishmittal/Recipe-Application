using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the change password command.
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ChangePasswordCommandHandler class.
    /// </summary>
    public ChangePasswordCommandHandler(
        RecipeDbContext db,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user is null)
            {
                return ApiResponse<bool>.Fail("User not found.", 404, nameof(ChangePasswordCommand));
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<bool>.Fail("Current password is incorrect.", 400, nameof(ChangePasswordCommand));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Password changed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(ChangePasswordCommandHandler));
            return ApiResponse<bool>.Fail("Password change failed due to an unexpected error.", 500, nameof(ChangePasswordCommand));
        }
    }
}
