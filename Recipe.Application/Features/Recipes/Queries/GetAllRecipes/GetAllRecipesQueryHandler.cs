using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Application.Features.Common;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the get all recipes query.
/// </summary>
public class GetAllRecipesQueryHandler : IRequestHandler<GetAllRecipesQuery, ApiResponse<PagedResponse<RecipeResponse>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetAllRecipesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetAllRecipesQueryHandler class.
    /// </summary>
    public GetAllRecipesQueryHandler(
        RecipeDbContext db,
        ILogger<GetAllRecipesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<PagedResponse<RecipeResponse>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var page = request.Request.Page < 1 ? 1 : request.Request.Page;
            var pageSize = request.Request.PageSize < 1 ? 10 : request.Request.PageSize;
            var sortBy = request.Request.SortBy?.Trim().ToLowerInvariant();
            var isAsc = string.Equals(request.Request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

            var query = _db.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeRatings)
                .Include(r => r.RecipeComments).ThenInclude(c => c.User)
                .Where(r => r.IsPublished)
                .AsQueryable();

            var search = request.Request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keywords = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(r => keywords.Any(k =>
                    r.Title.Contains(k) ||
                    r.Description.Contains(k) ||
                    r.Ingredients.Contains(k) ||
                    r.Steps.Contains(k) ||
                    (r.Category != null && r.Category.Name.Contains(k)) ||
                    r.RecipeTags.Any(rt => rt.Tag.Name.Contains(k)) ||
                    r.User.Name.Contains(k)));
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Category))
            {
                var category = request.Request.Category.Trim();
                query = query.Where(r => r.Category != null && r.Category.Name == category);
            }

            if (!string.IsNullOrWhiteSpace(request.Request.Tag))
            {
                var tag = request.Request.Tag.Trim();
                query = query.Where(r => r.RecipeTags.Any(rt => rt.Tag.Name == tag));
            }

            query = sortBy switch
            {
                "title" => isAsc ? query.OrderBy(r => r.Title) : query.OrderByDescending(r => r.Title),
                "preptime" => isAsc ? query.OrderBy(r => r.PrepTimeMinutes) : query.OrderByDescending(r => r.PrepTimeMinutes),
                "cooktime" => isAsc ? query.OrderBy(r => r.CookTimeMinutes) : query.OrderByDescending(r => r.CookTimeMinutes),
                "created" or _ => isAsc ? query.OrderBy(r => r.Id) : query.OrderByDescending(r => r.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResponse<RecipeResponse>
            {
                Items = items.Select(recipe => RecipeResponseMapper.ToResponse(recipe)).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<RecipeResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetAllRecipesQueryHandler));
            return ApiResponse<PagedResponse<RecipeResponse>>.Fail("Recipe listing failed due to an unexpected error.", 500, nameof(GetAllRecipesQuery));
        }
    }

}
