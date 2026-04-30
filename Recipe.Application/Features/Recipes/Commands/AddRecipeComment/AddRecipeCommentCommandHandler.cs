using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles recipe comment creation.
/// </summary>
public class AddRecipeCommentCommandHandler : IRequestHandler<AddRecipeCommentCommand, ApiResponse<RecipeCommentResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<AddRecipeCommentCommandHandler> _logger;

    public AddRecipeCommentCommandHandler(RecipeDbContext db, ILogger<AddRecipeCommentCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<RecipeCommentResponse>> Handle(AddRecipeCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            var recipeExists = await _db.Recipes.AnyAsync(r => r.Id == request.RecipeId, cancellationToken);

            if (user is null || !recipeExists)
            {
                return ApiResponse<RecipeCommentResponse>.Fail("Recipe or user not found.", 404, nameof(AddRecipeCommentCommand));
            }

            var comment = new RecipeComment
            {
                RecipeId = request.RecipeId,
                UserId = request.UserId,
                Content = request.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                User = user
            };

            _db.RecipeComments.Add(comment);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<RecipeCommentResponse>.Ok(ToResponse(comment), "Comment added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(AddRecipeCommentCommandHandler));
            return ApiResponse<RecipeCommentResponse>.Fail("Comment creation failed due to an unexpected error.", 500, nameof(AddRecipeCommentCommand));
        }
    }

    private static RecipeCommentResponse ToResponse(RecipeComment comment) =>
        new()
        {
            Id = comment.Id,
            UserId = comment.UserId,
            UserName = comment.User?.Name ?? string.Empty,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
}
