using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the command used to add favorite.
/// </summary>
public record AddFavoriteCommand(Guid UserId, Guid RecipeId) : IRequest<ApiResponse<bool>>;
