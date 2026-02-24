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
    public RecipeService(
        IRecipeRepository recipeRepo,
        IImageService imageService,
        IMapper mapper,
        ILogger<RecipeService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _recipeRepo = recipeRepo;
        _imageService = imageService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<RecipeResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching recipes page:");

        var recipes = await _recipeRepo.GetAllAsync();

        return _mapper.Map<List<RecipeResponse>>(recipes);
    }

    public async Task<List<RecipeResponse>> GetMyRecipesAsync()
    {
        var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _logger.LogInformation("Fetching recipes for userId: {UserId}", userId);
        var recipes = await _recipeRepo.GetMyRecipesAsync(userId); 
        _logger.LogInformation("Found {Count} recipes for userId: {UserId}", recipes.Count, userId);
        return _mapper.Map<List<RecipeResponse>>(recipes);
    }
    public async Task<RecipeResponse?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching recipe id: {Id}", id);
        var recipe = await _recipeRepo.GetByIdAsync(id);
        if (recipe is null)
        {
            _logger.LogWarning("Recipe not found with id: {Id}", id);
            return null;
        }
        return _mapper.Map<RecipeResponse>(recipe);
    }

    public async Task<RecipeResponse> CreateAsync(int userId, CreateRecipeRequest request)
    {
        _logger.LogInformation("Creating recipe for userId: {UserId}", userId);

        var recipe = _mapper.Map<Recipe>(request);
        recipe.UserId = userId;

        // Upload image if it is provided by the user
        if (request.Image is not null && request.Image.Length > 0)
        {
            _logger.LogInformation("Uploading image for new recipe");
            recipe.ImageUrl = await _imageService.UploadImageAsync(request.Image);
        }

        var created = await _recipeRepo.CreateAsync(recipe);
        _logger.LogInformation("Recipe created with id: {Id}", created.Id);
        return _mapper.Map<RecipeResponse>(created);
    }

    public async Task<RecipeResponse?> UpdateAsync(int id, int userId, UpdateRecipeRequest request)
    {
        _logger.LogInformation("Updating recipe id: {Id}", id);

        var recipe = await _recipeRepo.GetByIdAsync(id);
        if (recipe is null || recipe.UserId != userId)
        {
            _logger.LogWarning("Recipe not found or unauthorized id: {Id}", id);
            return null;
        }

        _mapper.Map(request, recipe);

        // Upload new image if provided
        //if (request.Image is not null && request.Image.Length > 0)
        //{
        //    // Delete old image first
        //    if (!string.IsNullOrEmpty(recipe.ImageUrl))
        //        await _imageService.DeleteImageAsync(recipe.ImageUrl);

        //    _logger.LogInformation("Uploading new image for recipe id: {Id}", id);
        //    recipe.ImageUrl = await _imageService.UploadImageAsync(request.Image);
        //}

        var updated = await _recipeRepo.UpdateAsync(recipe);
        _logger.LogInformation("Recipe updated with id: {Id}", id);
        return _mapper.Map<RecipeResponse>(updated);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        _logger.LogInformation("Deleting recipe id: {Id}", id);
        var result = await _recipeRepo.DeleteAsync(id, userId);
        if (!result) _logger.LogWarning("Recipe not found or unauthorized for id: {Id}", id);
        return result;
    }
}