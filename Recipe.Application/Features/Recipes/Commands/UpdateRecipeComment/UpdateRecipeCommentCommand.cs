using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests update of a recipe comment.
/// </summary>
public record UpdateRecipeCommentCommand(Guid CommentId, Guid UserId, bool IsAdmin, string Content) : IRequest<ApiResponse<RecipeCommentResponse>>;
