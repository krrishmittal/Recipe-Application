using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles recipe comment deletion.
/// </summary>
public class DeleteRecipeCommentCommandHandler : IRequestHandler<DeleteRecipeCommentCommand, ApiResponse<bool>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<DeleteRecipeCommentCommandHandler> _logger;

    public DeleteRecipeCommentCommandHandler(RecipeDbContext db, ILogger<DeleteRecipeCommentCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteRecipeCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _db.RecipeComments.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);
            if (comment is null || (!request.IsAdmin && comment.UserId != request.UserId))
            {
                return ApiResponse<bool>.Fail("Comment not found or access denied.", 404, nameof(DeleteRecipeCommentCommand));
            }

            _db.RecipeComments.Remove(comment);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Comment deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(DeleteRecipeCommentCommandHandler));
            return ApiResponse<bool>.Fail("Comment deletion failed due to an unexpected error.", 500, nameof(DeleteRecipeCommentCommand));
        }
    }
}
