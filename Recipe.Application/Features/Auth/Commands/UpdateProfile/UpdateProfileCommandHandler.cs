using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the update profile command.
/// </summary>
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResponse<UserProfileResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateProfileCommandHandler class.
    /// </summary>
    public UpdateProfileCommandHandler(
        RecipeDbContext db,
        IImageService imageService,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<UserProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _db.Users
                .Include(u => u.Recipes)
                .Include(u => u.FavoriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return ApiResponse<UserProfileResponse>.Fail("User not found.", 404, nameof(UpdateProfileCommand));
            }

            user.Name = request.Request.Name.Trim();
            user.Bio = string.IsNullOrWhiteSpace(request.Request.Bio) ? null : request.Request.Bio.Trim();

            if (request.Request.ProfileImage is not null && request.Request.ProfileImage.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                {
                    await _imageService.DeleteImageAsync(user.ProfileImageUrl);
                }

                user.ProfileImageUrl = await _imageService.UploadImageAsync(request.Request.ProfileImage);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<UserProfileResponse>.Ok(ToResponse(user), "Profile updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(UpdateProfileCommandHandler));
            return ApiResponse<UserProfileResponse>.Fail("Profile update failed due to an unexpected error.", 500, nameof(UpdateProfileCommand));
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
