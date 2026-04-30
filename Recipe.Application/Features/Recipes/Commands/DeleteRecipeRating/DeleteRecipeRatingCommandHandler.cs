using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles recipe rating deletion.
/// </summary>
public class DeleteRecipeRatingCommandHandler : IRequestHandler<DeleteRecipeRatingCommand, ApiResponse<RecipeRatingResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<DeleteRecipeRatingCommandHandler> _logger;

    public DeleteRecipeRatingCommandHandler(RecipeDbContext db, ILogger<DeleteRecipeRatingCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<RecipeRatingResponse>> Handle(DeleteRecipeRatingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rating = await _db.RecipeRatings.FirstOrDefaultAsync(
                r => r.RecipeId == request.RecipeId && r.UserId == request.UserId,
                cancellationToken);

            if (rating is null)
            {
                return ApiResponse<RecipeRatingResponse>.Fail("Rating not found.", 404, nameof(DeleteRecipeRatingCommand));
            }

            _db.RecipeRatings.Remove(rating);
            await _db.SaveChangesAsync(cancellationToken);

            var ratings = await _db.RecipeRatings.Where(r => r.RecipeId == request.RecipeId).ToListAsync(cancellationToken);
            return ApiResponse<RecipeRatingResponse>.Ok(new RecipeRatingResponse
            {
                RecipeId = request.RecipeId,
                UserRating = null,
                AverageRating = ratings.Count == 0 ? 0 : Math.Round(ratings.Average(r => r.Value), 2),
                RatingCount = ratings.Count
            }, "Rating removed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(DeleteRecipeRatingCommandHandler));
            return ApiResponse<RecipeRatingResponse>.Fail("Recipe rating deletion failed due to an unexpected error.", 500, nameof(DeleteRecipeRatingCommand));
        }
    }
}
