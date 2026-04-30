using Recipe.Application.DTOs.Response;
using Recipe.Domain.Models;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Common;

/// <summary>
/// Maps recipe entities to API response models.
/// </summary>
public static class RecipeResponseMapper
{
    /// <summary>
    /// Maps a recipe entity to a response model.
    /// </summary>
    public static RecipeResponse ToResponse(RecipeEntity recipe, bool includeComments = false) =>
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
            UserId = recipe.UserId,
            IsPublished = recipe.IsPublished,
            IsFeatured = recipe.IsFeatured,
            AuthorName = recipe.User?.Name ?? string.Empty,
            Category = recipe.Category?.Name,
            Tags = recipe.RecipeTags.Select(rt => rt.Tag.Name).OrderBy(n => n).ToList(),
            AverageRating = recipe.RecipeRatings.Count == 0 ? 0 : Math.Round(recipe.RecipeRatings.Average(r => r.Value), 2),
            RatingCount = recipe.RecipeRatings.Count,
            Comments = includeComments
                ? recipe.RecipeComments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new RecipeCommentResponse
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User?.Name ?? string.Empty,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToList()
                : recipe.RecipeComments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(3)
                    .Select(c => new RecipeCommentResponse
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User?.Name ?? string.Empty,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToList()
        };
}
