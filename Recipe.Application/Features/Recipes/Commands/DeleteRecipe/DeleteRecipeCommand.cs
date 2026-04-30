using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Represents the command used to delete recipe.
/// </summary>
public record DeleteRecipeCommand(Guid Id, Guid UserId, bool IsAdmin) : IRequest<ApiResponse<bool>>;
