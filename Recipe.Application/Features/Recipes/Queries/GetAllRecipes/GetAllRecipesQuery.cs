using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the query used to get all recipes.
/// </summary>
public record GetAllRecipesQuery(PagedRequest Request) : IRequest<ApiResponse<PagedResponse<RecipeResponse>>>;
