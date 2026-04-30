using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the command used to create recipe.
/// </summary>
public record CreateRecipeCommand(Guid UserId, CreateRecipeRequest Request) : IRequest<ApiResponse<RecipeResponse>>;
