using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles recipe comment updates.
/// </summary>
public class UpdateRecipeCommentCommandHandler : IRequestHandler<UpdateRecipeCommentCommand, ApiResponse<RecipeCommentResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<UpdateRecipeCommentCommandHandler> _logger;

    public UpdateRecipeCommentCommandHandler(RecipeDbContext db, ILogger<UpdateRecipeCommentCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<RecipeCommentResponse>> Handle(UpdateRecipeCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _db.RecipeComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

            if (comment is null || (!request.IsAdmin && comment.UserId != request.UserId))
            {
                return ApiResponse<RecipeCommentResponse>.Fail("Comment not found or access denied.", 404, nameof(UpdateRecipeCommentCommand));
            }

            comment.Content = request.Content.Trim();
            comment.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<RecipeCommentResponse>.Ok(new RecipeCommentResponse
            {
                Id = comment.Id,
                UserId = comment.UserId,
                UserName = comment.User?.Name ?? string.Empty,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            }, "Comment updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(UpdateRecipeCommentCommandHandler));
            return ApiResponse<RecipeCommentResponse>.Fail("Comment update failed due to an unexpected error.", 500, nameof(UpdateRecipeCommentCommand));
        }
    }
}
