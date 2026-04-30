using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Handles admin user deletion requests.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the DeleteUserCommandHandler class.
    /// </summary>
    public DeleteUserCommandHandler(
        RecipeDbContext db,
        IImageService imageService,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _db.Users
                .Include(u => u.Recipes)
                .Include(u => u.FavoriteRecipes)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return ApiResponse<bool>.Fail("User not found.", 404, nameof(DeleteUserCommand));
            }

            if (user.Role == UserRoles.Admin)
            {
                var adminCount = await _db.Users.CountAsync(u => u.Role == UserRoles.Admin, cancellationToken);
                if (adminCount <= 1)
                {
                    return ApiResponse<bool>.Fail("The last admin account cannot be deleted.", 400, nameof(DeleteUserCommand));
                }
            }

            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            {
                await _imageService.DeleteImageAsync(user.ProfileImageUrl);
            }

            foreach (var recipe in user.Recipes.Where(r => !string.IsNullOrWhiteSpace(r.ImageUrl)))
            {
                await _imageService.DeleteImageAsync(recipe.ImageUrl!);
            }

            if (user.FavoriteRecipes.Count > 0)
            {
                _db.FavoriteRecipes.RemoveRange(user.FavoriteRecipes);
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "User deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(DeleteUserCommandHandler));
            return ApiResponse<bool>.Fail("User deletion failed due to an unexpected error.", 500, nameof(DeleteUserCommand));
        }
    }
}
