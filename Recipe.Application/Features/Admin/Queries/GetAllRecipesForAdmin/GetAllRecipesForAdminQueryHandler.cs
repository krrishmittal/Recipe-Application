using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Handles admin recipe list requests.
/// </summary>
public class GetAllRecipesForAdminQueryHandler : IRequestHandler<GetAllRecipesForAdminQuery, ApiResponse<PagedResponse<RecipeResponse>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetAllRecipesForAdminQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetAllRecipesForAdminQueryHandler class.
    /// </summary>
    public GetAllRecipesForAdminQueryHandler(
        RecipeDbContext db,
        ILogger<GetAllRecipesForAdminQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<PagedResponse<RecipeResponse>>> Handle(GetAllRecipesForAdminQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var page = request.Request.Page;
            var pageSize = request.Request.PageSize;
            var search = request.Request.Search?.Trim();
            var sortBy = request.Request.SortBy?.Trim().ToLowerInvariant();
            var isAsc = string.Equals(request.Request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

            var query = _db.Recipes
                .AsNoTracking()
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Title.Contains(search) ||
                    r.Description.Contains(search) ||
                    r.User.Name.Contains(search));
            }

            query = sortBy switch
            {
                "title" => isAsc ? query.OrderBy(r => r.Title) : query.OrderByDescending(r => r.Title),
                "author" => isAsc ? query.OrderBy(r => r.User.Name) : query.OrderByDescending(r => r.User.Name),
                "published" => isAsc ? query.OrderBy(r => r.IsPublished) : query.OrderByDescending(r => r.IsPublished),
                "featured" => isAsc ? query.OrderBy(r => r.IsFeatured) : query.OrderByDescending(r => r.IsFeatured),
                _ => isAsc ? query.OrderBy(r => r.Id) : query.OrderByDescending(r => r.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var response = new PagedResponse<RecipeResponse>
            {
                Items = items.Select(ToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<RecipeResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetAllRecipesForAdminQueryHandler));
            return ApiResponse<PagedResponse<RecipeResponse>>.Fail("Admin recipe listing failed due to an unexpected error.", 500, nameof(GetAllRecipesForAdminQuery));
        }
    }

    private static RecipeResponse ToResponse(RecipeEntity recipe) =>
        new()
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            ImageUrl = recipe.ImageUrl,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Ingredients = recipe.Ingredients,
            Steps = recipe.Steps,
            AuthorName = recipe.User?.Name ?? string.Empty,
            UserId = recipe.UserId,
            IsPublished = recipe.IsPublished,
            IsFeatured = recipe.IsFeatured
        };
}
