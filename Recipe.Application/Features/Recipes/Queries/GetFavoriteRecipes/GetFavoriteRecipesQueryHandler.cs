using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the get favorite recipes query.
/// </summary>
public class GetFavoriteRecipesQueryHandler : IRequestHandler<GetFavoriteRecipesQuery, ApiResponse<PagedResponse<FavoriteRecipeResponse>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetFavoriteRecipesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetFavoriteRecipesQueryHandler class.
    /// </summary>
    public GetFavoriteRecipesQueryHandler(
        RecipeDbContext db,
        ILogger<GetFavoriteRecipesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<PagedResponse<FavoriteRecipeResponse>>> Handle(GetFavoriteRecipesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var page = request.Request.Page < 1 ? 1 : request.Request.Page;
            var pageSize = request.Request.PageSize < 1 ? 10 : request.Request.PageSize;

            var query = _db.FavoriteRecipes
                .Include(f => f.Recipe)
                .ThenInclude(r => r.User)
                .Where(f => f.UserId == request.UserId && f.Recipe.IsPublished)
                .OrderByDescending(f => f.CreatedAt)
                .AsQueryable();

            var search = request.Request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f =>
                    f.Recipe.Title.Contains(search) ||
                    f.Recipe.Description.Contains(search) ||
                    f.Recipe.User.Name.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResponse<FavoriteRecipeResponse>
            {
                Items = items.Select(f => new FavoriteRecipeResponse
                {
                    RecipeId = f.RecipeId,
                    Title = f.Recipe.Title,
                    Description = f.Recipe.Description,
                    ImageUrl = f.Recipe.ImageUrl,
                    AuthorName = f.Recipe.User?.Name ?? string.Empty,
                    FavoritedAt = f.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<FavoriteRecipeResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetFavoriteRecipesQueryHandler));
            return ApiResponse<PagedResponse<FavoriteRecipeResponse>>.Fail("Favorite recipe listing failed due to an unexpected error.", 500, nameof(GetFavoriteRecipesQuery));
        }
    }
}
