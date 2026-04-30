using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the remove favorite command.
/// </summary>
public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<RemoveFavoriteCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the RemoveFavoriteCommandHandler class.
    /// </summary>
    public RemoveFavoriteCommandHandler(
        RecipeDbContext db,
        ILogger<RemoveFavoriteCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var favorite = await _db.FavoriteRecipes.FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.RecipeId == request.RecipeId,
                cancellationToken);

            if (favorite is null)
            {
                return ApiResponse<bool>.Fail("Favorite recipe not found.", 404, nameof(RemoveFavoriteCommand));
            }

            _db.FavoriteRecipes.Remove(favorite);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Recipe removed from favorites.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(RemoveFavoriteCommandHandler));
            return ApiResponse<bool>.Fail("Removing favorite failed due to an unexpected error.", 500, nameof(RemoveFavoriteCommand));
        }
    }
}
