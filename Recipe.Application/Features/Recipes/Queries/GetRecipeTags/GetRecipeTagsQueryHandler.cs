using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles tag list requests.
/// </summary>
public class GetRecipeTagsQueryHandler : IRequestHandler<GetRecipeTagsQuery, ApiResponse<List<string>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetRecipeTagsQueryHandler> _logger;

    public GetRecipeTagsQueryHandler(RecipeDbContext db, ILogger<GetRecipeTagsQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApiResponse<List<string>>> Handle(GetRecipeTagsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tags = await _db.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .ToListAsync(cancellationToken);

            return ApiResponse<List<string>>.Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetRecipeTagsQueryHandler));
            return ApiResponse<List<string>>.Fail("Tag listing failed due to an unexpected error.", 500, nameof(GetRecipeTagsQuery));
        }
    }
}
