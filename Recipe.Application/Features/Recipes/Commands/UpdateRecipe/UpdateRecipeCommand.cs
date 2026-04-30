using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the command used to update recipe.
/// </summary>
public record UpdateRecipeCommand(Guid Id, Guid UserId, UpdateRecipeRequest Request) : IRequest<ApiResponse<RecipeResponse>>;
