using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests add or update of a recipe rating.
/// </summary>
public record RateRecipeCommand(Guid RecipeId, Guid UserId, int Value) : IRequest<ApiResponse<RecipeRatingResponse>>;
