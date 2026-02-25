using AutoMapper;
using Server.DTOs.Request;
using Server.DTOs.Response;
using Server.Models;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepo;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;
    private readonly ILogger<RecipeService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecipeService(IRecipeRepository recipeRepo, IImageService imageService,
        IMapper mapper, ILogger<RecipeService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepo = recipeRepo;
        _imageService = imageService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<PagedResponse<RecipeResponse>>> GetAllAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Fetching all recipes - Page: {Page}, Search: {Search}",
                request.Page, request.Search);

            var (recipes, totalCount) = await _recipeRepo.GetAllAsync(request);

            var result = new PagedResponse<RecipeResponse>
            {
                Items = _mapper.Map<List<RecipeResponse>>(recipes),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResponse<RecipeResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(GetAllAsync));
            return ApiResponse<PagedResponse<RecipeResponse>>.Fail("Something went wrong.", 500, nameof(GetAllAsync));
        }
    }

    public async Task<ApiResponse<PagedResponse<RecipeResponse>>> GetMyRecipesAsync(PagedRequest request)
    {
        try
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);

            _logger.LogInformation("Fetching recipes for userId: {UserId} - Page: {Page}, Search: {Search}",
                userId, request.Page, request.Search);

            var (recipes, totalCount) = await _recipeRepo.GetMyRecipesAsync(userId, request);

            _logger.LogInformation("Found {Count} total recipes for userId: {UserId}", totalCount, userId);

            var result = new PagedResponse<RecipeResponse>
            {
                Items = _mapper.Map<List<RecipeResponse>>(recipes),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResponse<RecipeResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(GetMyRecipesAsync));
            return ApiResponse<PagedResponse<RecipeResponse>>.Fail("Something went wrong.", 500, nameof(GetMyRecipesAsync));
        }
    }


    public async Task<ApiResponse<RecipeResponse>> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching recipe id: {Id}", id);
            var recipe = await _recipeRepo.GetByIdAsync(id);

            if (recipe is null)
            {
                _logger.LogWarning("Recipe not found with id: {Id}", id);
                return ApiResponse<RecipeResponse>.Fail("Recipe not found.", 404, nameof(GetByIdAsync));
            }

            return ApiResponse<RecipeResponse>.Ok(_mapper.Map<RecipeResponse>(recipe));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(GetByIdAsync));
            return ApiResponse<RecipeResponse>.Fail("Something went wrong.", 500, nameof(GetByIdAsync));
        }
    }

    public async Task<ApiResponse<RecipeResponse>> CreateAsync(int userId, CreateRecipeRequest request)
    {
        try
        {
            _logger.LogInformation("Creati ng recipe for userId: {UserId}", userId);

            var recipe = _mapper.Map<Recipe>(request);
            recipe.UserId = userId;

            if (request.Image is not null && request.Image.Length > 0)
            {
                _logger.LogInformation("Uploading image for new recipe");
                recipe.ImageUrl = await _imageService.UploadImageAsync(request.Image);
            }

            var created = await _recipeRepo.CreateAsync(recipe);
            _logger.LogInformation("Recipe created with id: {Id}", created.Id);

            return ApiResponse<RecipeResponse>.Ok(_mapper.Map<RecipeResponse>(created), "Recipe created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(CreateAsync));
            return ApiResponse<RecipeResponse>.Fail("Something went wrong.", 500, nameof(CreateAsync));
        }
    }

    public async Task<ApiResponse<RecipeResponse>> UpdateAsync(int id, int userId, UpdateRecipeRequest request)
    {
        try
        {
            _logger.LogInformation("Updating recipe id: {Id}", id);

            var recipe = await _recipeRepo.GetByIdAsync(id);
            if (recipe is null || recipe.UserId != userId)
            {
                _logger.LogWarning("Recipe not found or unauthorized id: {Id}", id);
                return ApiResponse<RecipeResponse>.Fail("Recipe not found or you are not the owner.", 404, nameof(UpdateAsync));
            }

            _mapper.Map(request, recipe);
            var updated = await _recipeRepo.UpdateAsync(recipe);
            _logger.LogInformation("Recipe updated with id: {Id}", id);

            return ApiResponse<RecipeResponse>.Ok(_mapper.Map<RecipeResponse>(updated), "Recipe updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(UpdateAsync));
            return ApiResponse<RecipeResponse>.Fail("Something went wrong.", 500, nameof(UpdateAsync));
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
    {
        try
        {
            _logger.LogInformation("Deleting recipe id: {Id}", id);
            var result = await _recipeRepo.DeleteAsync(id, userId);

            if (!result)
            {
                _logger.LogWarning("Recipe not found or unauthorized for id: {Id}", id);
                return ApiResponse<bool>.Fail("Recipe not found or you are not the owner.", 404, nameof(DeleteAsync));
            }

            return ApiResponse<bool>.Ok(true, "Recipe deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(DeleteAsync));
            return ApiResponse<bool>.Fail("Something went wrong.", 500, nameof(DeleteAsync));
        }
    }
}