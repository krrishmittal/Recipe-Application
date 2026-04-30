using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests creation of a recipe comment.
/// </summary>
public record AddRecipeCommentCommand(Guid RecipeId, Guid UserId, string Content) : IRequest<ApiResponse<RecipeCommentResponse>>;
