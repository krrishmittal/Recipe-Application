using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Handles admin recipe publish status updates.
/// </summary>
public class UpdateRecipePublishStatusCommandHandler : IRequestHandler<UpdateRecipePublishStatusCommand, ApiResponse<RecipeResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<UpdateRecipePublishStatusCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateRecipePublishStatusCommandHandler class.
    /// </summary>
    public UpdateRecipePublishStatusCommandHandler(
        RecipeDbContext db,
        ILogger<UpdateRecipePublishStatusCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<RecipeResponse>> Handle(UpdateRecipePublishStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _db.Recipes
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == request.RecipeId, cancellationToken);

            if (recipe is null)
            {
                return ApiResponse<RecipeResponse>.Fail("Recipe not found.", 404, nameof(UpdateRecipePublishStatusCommand));
            }

            recipe.IsPublished = request.IsPublished;
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<RecipeResponse>.Ok(ToResponse(recipe), "Recipe publish status updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(UpdateRecipePublishStatusCommandHandler));
            return ApiResponse<RecipeResponse>.Fail("Recipe publish status update failed due to an unexpected error.", 500, nameof(UpdateRecipePublishStatusCommand));
        }
    }

    private static RecipeResponse ToResponse(RecipeEntity recipe) =>
        new()
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            ImageUrl = recipe.ImageUrl,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Ingredients = recipe.Ingredients,
            Steps = recipe.Steps,
            AuthorName = recipe.User?.Name ?? string.Empty,
            UserId = recipe.UserId,
            IsPublished = recipe.IsPublished,
            IsFeatured = recipe.IsFeatured
        };
}
