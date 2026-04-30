using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Application.DTOs.Response;
using Recipe.Infrastructure.Models;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Handles admin user list requests.
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<PagedResponse<UserProfileResponse>>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetAllUsersQueryHandler class.
    /// </summary>
    public GetAllUsersQueryHandler(
        RecipeDbContext db,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<PagedResponse<UserProfileResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var page = request.Request.Page;
            var pageSize = request.Request.PageSize;
            var search = request.Request.Search?.Trim();
            var sortBy = request.Request.SortBy?.Trim().ToLowerInvariant();
            var isAsc = string.Equals(request.Request.SortOrder, "asc", StringComparison.OrdinalIgnoreCase);

            var query = _db.Users
                .AsNoTracking()
                .Include(u => u.Recipes)
                .Include(u => u.FavoriteRecipes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Role.Contains(search));
            }

            query = sortBy switch
            {
                "name" => isAsc ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
                "email" => isAsc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "role" => isAsc ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role),
                _ => isAsc ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var response = new PagedResponse<UserProfileResponse>
            {
                Items = items.Select(ToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<UserProfileResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetAllUsersQueryHandler));
            return ApiResponse<PagedResponse<UserProfileResponse>>.Fail("Admin user listing failed due to an unexpected error.", 500, nameof(GetAllUsersQuery));
        }
    }

    private static UserProfileResponse ToResponse(Recipe.Domain.Models.User user) =>
        new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            CreatedAt = user.CreatedAt,
            RecipeCount = user.Recipes.Count,
            FavoriteCount = user.FavoriteRecipes.Count
        };
}
