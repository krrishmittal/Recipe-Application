using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the query used to get recipe by id.
/// </summary>
public record GetRecipeByIdQuery(Guid Id) : IRequest<ApiResponse<RecipeResponse>>;
