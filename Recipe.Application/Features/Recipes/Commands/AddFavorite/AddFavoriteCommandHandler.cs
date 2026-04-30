using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the add favorite command.
/// </summary>
public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<AddFavoriteCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the AddFavoriteCommandHandler class.
    /// </summary>
    public AddFavoriteCommandHandler(
        RecipeDbContext db,
        ILogger<AddFavoriteCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipeExists = await _db.Recipes.AnyAsync(r => r.Id == request.RecipeId, cancellationToken);
            if (!recipeExists)
            {
                return ApiResponse<bool>.Fail("Recipe not found.", 404, nameof(AddFavoriteCommand));
            }

            var favoriteExists = await _db.FavoriteRecipes.AnyAsync(
                f => f.UserId == request.UserId && f.RecipeId == request.RecipeId,
                cancellationToken);

            if (favoriteExists)
            {
                return ApiResponse<bool>.Ok(true, "Recipe is already in favorites.");
            }

            _db.FavoriteRecipes.Add(new FavoriteRecipe
            {
                UserId = request.UserId,
                RecipeId = request.RecipeId,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.Ok(true, "Recipe added to favorites.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(AddFavoriteCommandHandler));
            return ApiResponse<bool>.Fail("Adding favorite failed due to an unexpected error.", 500, nameof(AddFavoriteCommand));
        }
    }
}
