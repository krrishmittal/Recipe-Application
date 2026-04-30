using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Application.Features.Common;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the update recipe command.
/// </summary>
public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, ApiResponse<RecipeResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<UpdateRecipeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateRecipeCommandHandler class.
    /// </summary>
    public UpdateRecipeCommandHandler(
        RecipeDbContext db,
        IImageService imageService,
        ILogger<UpdateRecipeCommandHandler> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<RecipeResponse>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _db.Recipes
                .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeComments)
                .ThenInclude(c => c.User)
                .Include(r => r.RecipeRatings)
                .Include(r => r.Category)
                .Include(r => r.User)
                .FirstOrDefaultAsync(
                r => r.Id == request.Id && r.UserId == request.UserId,
                cancellationToken);

            if (recipe is null)
            {
                return ApiResponse<RecipeResponse>.Fail("Recipe not found or you are not the owner.", 404, nameof(UpdateRecipeCommand));
            }

            recipe.Title = request.Request.Title;
            recipe.Description = request.Request.Description;
            recipe.PrepTimeMinutes = request.Request.PrepTimeMinutes;
            recipe.CookTimeMinutes = request.Request.CookTimeMinutes;
            recipe.Ingredients = request.Request.Ingredients;
            recipe.Steps = request.Request.steps;
            recipe.Category = await GetOrCreateCategoryAsync(request.Request.Category, cancellationToken);

            if (request.Request.Image is not null && request.Request.Image.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(recipe.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(recipe.ImageUrl);
                }

                recipe.ImageUrl = await _imageService.UploadImageAsync(request.Request.Image);
            }

            await SyncTagsAsync(recipe, request.Request.Tags, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<RecipeResponse>.Ok(RecipeResponseMapper.ToResponse(recipe), "Recipe updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(UpdateRecipeCommandHandler));
            return ApiResponse<RecipeResponse>.Fail("Recipe update failed due to an unexpected error.", 500, nameof(UpdateRecipeCommand));
        }
    }

    private async Task<Category?> GetOrCreateCategoryAsync(string? categoryName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return null;
        }

        var normalized = categoryName.Trim();
        var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Name == normalized, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var category = new Category { Name = normalized };
        _db.Categories.Add(category);
        return category;
    }

    private async Task SyncTagsAsync(RecipeEntity recipe, string? tags, CancellationToken cancellationToken)
    {
        if (recipe.RecipeTags.Count > 0)
        {
            _db.RecipeTags.RemoveRange(recipe.RecipeTags);
            recipe.RecipeTags.Clear();
        }

        foreach (var tagName in ParseTags(tags))
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken)
                ?? new Tag { Name = tagName };

            if (_db.Entry(tag).State == EntityState.Detached)
            {
                _db.Tags.Add(tag);
            }

            recipe.RecipeTags.Add(new RecipeTag
            {
                RecipeId = recipe.Id,
                TagId = tag.Id,
                Recipe = recipe,
                Tag = tag
            });
        }
    }

    private static List<string> ParseTags(string? tags) =>
        string.IsNullOrWhiteSpace(tags)
            ? new List<string>()
            : tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
}
