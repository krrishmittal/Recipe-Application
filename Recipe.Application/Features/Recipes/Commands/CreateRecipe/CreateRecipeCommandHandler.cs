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
/// Handles the create recipe command.
/// </summary>
public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, ApiResponse<RecipeResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<CreateRecipeCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the CreateRecipeCommandHandler class.
    /// </summary>
    public CreateRecipeCommandHandler(
        RecipeDbContext db,
        IImageService imageService,
        ILogger<CreateRecipeCommandHandler> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<RecipeResponse>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await GetOrCreateCategoryAsync(request.Request.Category, cancellationToken);
            var recipe = new RecipeEntity
            {
                Title = request.Request.Title,
                Description = request.Request.Description,
                PrepTimeMinutes = request.Request.PrepTimeMinutes,
                CookTimeMinutes = request.Request.CookTimeMinutes,
                Ingredients = request.Request.Ingredients,
                Steps = request.Request.steps,
                UserId = request.UserId,
                Category = category
            };

            if (request.Request.Image is not null && request.Request.Image.Length > 0)
            {
                recipe.ImageUrl = await _imageService.UploadImageAsync(request.Request.Image);
            }

            _db.Recipes.Add(recipe);
            await SyncTagsAsync(recipe, request.Request.Tags, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _db.Entry(recipe).Reference(r => r.User).LoadAsync(cancellationToken);
            await _db.Entry(recipe).Reference(r => r.Category).LoadAsync(cancellationToken);
            await _db.Entry(recipe).Collection(r => r.RecipeTags).Query().Include(rt => rt.Tag).LoadAsync(cancellationToken);
            await _db.Entry(recipe).Collection(r => r.RecipeRatings).LoadAsync(cancellationToken);
            await _db.Entry(recipe).Collection(r => r.RecipeComments).LoadAsync(cancellationToken);

            return ApiResponse<RecipeResponse>.Ok(RecipeResponseMapper.ToResponse(recipe), "Recipe created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(CreateRecipeCommandHandler));
            return ApiResponse<RecipeResponse>.Fail("Recipe creation failed due to an unexpected error.", 500, nameof(CreateRecipeCommand));
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
        foreach (var tagName in ParseTags(tags))
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken)
                ?? new Tag { Name = tagName };

            if (tag.Id == Guid.Empty)
            {
                tag.Id = Guid.NewGuid();
            }

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
