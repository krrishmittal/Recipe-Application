using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles recipe rating requests.
/// </summary>
public class RateRecipeCommandHandler : IRequestHandler<RateRecipeCommand, ApiResponse<RecipeRatingResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<RateRecipeCommandHandler> _logger;

    public RateRecipeCommandHandler(RecipeDbContext db, ILogger<RateRecipeCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<RecipeRatingResponse>> Handle(RateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipeExists = await _db.Recipes.AnyAsync(r => r.Id == request.RecipeId, cancellationToken);
            if (!recipeExists)
            {
                return ApiResponse<RecipeRatingResponse>.Fail("Recipe not found.", 404, nameof(RateRecipeCommand));
            }

            var rating = await _db.RecipeRatings.FirstOrDefaultAsync(
                r => r.RecipeId == request.RecipeId && r.UserId == request.UserId,
                cancellationToken);

            if (rating is null)
            {
                rating = new RecipeRating
                {
                    RecipeId = request.RecipeId,
                    UserId = request.UserId,
                    Value = request.Value,
                    CreatedAt = DateTime.UtcNow
                };

                _db.RecipeRatings.Add(rating);
            }
            else
            {
                rating.Value = request.Value;
                rating.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<RecipeRatingResponse>.Ok(
                await BuildSummaryAsync(request.RecipeId, request.UserId, cancellationToken),
                "Rating saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(RateRecipeCommandHandler));
            return ApiResponse<RecipeRatingResponse>.Fail("Recipe rating failed due to an unexpected error.", 500, nameof(RateRecipeCommand));
        }
    }

    private async Task<RecipeRatingResponse> BuildSummaryAsync(Guid recipeId, Guid userId, CancellationToken cancellationToken)
    {
        var ratings = await _db.RecipeRatings
            .Where(r => r.RecipeId == recipeId)
            .ToListAsync(cancellationToken);

        return new RecipeRatingResponse
        {
            RecipeId = recipeId,
            UserRating = ratings.FirstOrDefault(r => r.UserId == userId)?.Value,
            AverageRating = ratings.Count == 0 ? 0 : Math.Round(ratings.Average(r => r.Value), 2),
            RatingCount = ratings.Count
        };
    }
}
