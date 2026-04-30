using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Requests a featured status update for a recipe.
/// </summary>
public record UpdateRecipeFeaturedStatusCommand(Guid RecipeId, bool IsFeatured) : IRequest<ApiResponse<RecipeResponse>>;
