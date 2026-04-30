using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Requests a publish status update for a recipe.
/// </summary>
public record UpdateRecipePublishStatusCommand(Guid RecipeId, bool IsPublished) : IRequest<ApiResponse<RecipeResponse>>;
