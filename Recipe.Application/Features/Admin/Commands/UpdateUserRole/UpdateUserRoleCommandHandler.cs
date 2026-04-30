using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Handles admin user role updates.
/// </summary>
public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, ApiResponse<UserProfileResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<UpdateUserRoleCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateUserRoleCommandHandler class.
    /// </summary>
    public UpdateUserRoleCommandHandler(
        RecipeDbContext db,
        ILogger<UpdateUserRoleCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<UserProfileResponse>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var normalizedRole = request.Role.Trim();
            var user = await _db.Users
                .Include(u => u.Recipes)
                .Include(u => u.FavoriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return ApiResponse<UserProfileResponse>.Fail("User not found.", 404, nameof(UpdateUserRoleCommand));
            }

            if (!normalizedRole.Equals(UserRoles.User, StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<UserProfileResponse>.Fail("Admin role cannot be assigned through the API.", 403, nameof(UpdateUserRoleCommand));
            }

            if (user.Role == UserRoles.Admin)
            {
                var adminCount = await _db.Users.CountAsync(u => u.Role == UserRoles.Admin, cancellationToken);
                if (adminCount <= 1)
                {
                    return ApiResponse<UserProfileResponse>.Fail("The last admin cannot be demoted.", 400, nameof(UpdateUserRoleCommand));
                }
            }

            user.Role = UserRoles.User;
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<UserProfileResponse>.Ok(ToResponse(user), "User role updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(UpdateUserRoleCommandHandler));
            return ApiResponse<UserProfileResponse>.Fail("User role update failed due to an unexpected error.", 500, nameof(UpdateUserRoleCommand));
        }
    }

    private static UserProfileResponse ToResponse(Recipe.Domain.Models.User user) =>
        new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            CreatedAt = user.CreatedAt,
            RecipeCount = user.Recipes.Count,
            FavoriteCount = user.FavoriteRecipes.Count
        };
}
