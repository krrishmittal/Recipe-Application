using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the command used to remove favorite.
/// </summary>
public record RemoveFavoriteCommand(Guid UserId, Guid RecipeId) : IRequest<ApiResponse<bool>>;
