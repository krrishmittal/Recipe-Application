using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Application.Features.Common;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Handles the get recipe by id query.
/// </summary>
public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, ApiResponse<RecipeResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly ILogger<GetRecipeByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetRecipeByIdQueryHandler class.
    /// </summary>
    public GetRecipeByIdQueryHandler(
        RecipeDbContext db,
        ILogger<GetRecipeByIdQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<RecipeResponse>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _db.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeRatings)
                .Include(r => r.RecipeComments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.IsPublished, cancellationToken);

            if (recipe is null)
            {
                return ApiResponse<RecipeResponse>.Fail("Recipe not found.", 404, nameof(GetRecipeByIdQuery));
            }

            return ApiResponse<RecipeResponse>.Ok(RecipeResponseMapper.ToResponse(recipe, includeComments: true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(GetRecipeByIdQueryHandler));
            return ApiResponse<RecipeResponse>.Fail("Recipe lookup failed due to an unexpected error.", 500, nameof(GetRecipeByIdQuery));
        }
    }

}
