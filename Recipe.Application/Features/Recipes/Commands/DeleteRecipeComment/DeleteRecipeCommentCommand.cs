using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests deletion of a recipe comment.
/// </summary>
public record DeleteRecipeCommentCommand(Guid CommentId, Guid UserId, bool IsAdmin) : IRequest<ApiResponse<bool>>;
