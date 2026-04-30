using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles category list requests.
/// </summary>
public class GetRecipeCategoriesQueryHandler : IRequestHandler<GetRecipeCategoriesQuery, ApiResponse<List<string>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetRecipeCategoriesQueryHandler> _logger;

    public GetRecipeCategoriesQueryHandler(RecipeDbContext db, ILogger<GetRecipeCategoriesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<List<string>>> Handle(GetRecipeCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync(cancellationToken);

            return ApiResponse<List<string>>.Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetRecipeCategoriesQueryHandler));
            return ApiResponse<List<string>>.Fail("Category listing failed due to an unexpected error.", 500, nameof(GetRecipeCategoriesQuery));
        }
    }
}
