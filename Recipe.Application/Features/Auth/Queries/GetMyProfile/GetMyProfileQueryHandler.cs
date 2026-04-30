using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the get my profile query.
/// </summary>
public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ApiResponse<UserProfileResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetMyProfileQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetMyProfileQueryHandler class.
    /// </summary>
    public GetMyProfileQueryHandler(
        RecipeDbContext db,
        ILogger<GetMyProfileQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<UserProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _db.Users
                .Include(u => u.Recipes)
                .Include(u => u.FavoriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return ApiResponse<UserProfileResponse>.Fail("User not found.", 404, nameof(GetMyProfileQuery));
            }

            var response = new UserProfileResponse
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

            return ApiResponse<UserProfileResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetMyProfileQueryHandler));
            return ApiResponse<UserProfileResponse>.Fail("Profile lookup failed due to an unexpected error.", 500, nameof(GetMyProfileQuery));
        }
    }
}
