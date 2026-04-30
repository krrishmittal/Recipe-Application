using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the query used to get favorite recipes.
/// </summary>
public record GetFavoriteRecipesQuery(Guid UserId, PagedRequest Request) : IRequest<ApiResponse<PagedResponse<FavoriteRecipeResponse>>>;
