using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests deletion of the current user's rating on a recipe.
/// </summary>
public record DeleteRecipeRatingCommand(Guid RecipeId, Guid UserId) : IRequest<ApiResponse<RecipeRatingResponse>>;
