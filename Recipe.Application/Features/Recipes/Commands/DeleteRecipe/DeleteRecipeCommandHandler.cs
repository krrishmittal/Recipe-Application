using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the delete recipe command.
/// </summary>
public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<DeleteRecipeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the DeleteRecipeCommandHandler class.
    /// </summary>
    public DeleteRecipeCommandHandler(
        RecipeDbContext db,
        IImageService imageService,
        ILogger<DeleteRecipeCommandHandler> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<bool>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (recipe is null || (!request.IsAdmin && recipe.UserId != request.UserId))
            {
                return ApiResponse<bool>.Fail("Recipe not found or you are not the owner.", 404, nameof(DeleteRecipeCommand));
            }

            if (!string.IsNullOrWhiteSpace(recipe.ImageUrl))
            {
                await _imageService.DeleteImageAsync(recipe.ImageUrl);
            }

            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Recipe deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(DeleteRecipeCommandHandler));
            return ApiResponse<bool>.Fail("Recipe deletion failed due to an unexpected error.", 500, nameof(DeleteRecipeCommand));
        }
    }
}
