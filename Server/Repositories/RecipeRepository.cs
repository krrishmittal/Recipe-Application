using Microsoft.EntityFrameworkCore;
using Server.DTOs.Request;
using Server.Models;
using Server.Repositories.Interfaces;

namespace Server.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeDbContext _db;
        private readonly ILogger<RecipeRepository> _logger;
        public RecipeRepository(RecipeDbContext db, ILogger<RecipeRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<(List<Recipe> Items, int TotalCount)> GetAllAsync(PagedRequest request)
        {
            _logger.LogInformation("Getting all recipes - Page: {Page}, Search: {Search}", request.Page, request.Search);

            var query = _db.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keywords = request.Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(r => keywords.Any(k =>
                    r.Title.Contains(k) ||
                    r.Description.Contains(k) ||
                    r.Ingredients.Contains(k)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            _logger.LogInformation("Got {Count} of {Total} recipes", items.Count, totalCount);
            return (items, totalCount);
        }
        public async Task<(List<Recipe> Items, int TotalCount)> GetMyRecipesAsync(int userId, PagedRequest request)
        {
            _logger.LogInformation("Getting recipes for userId: {UserId} - Page: {Page}, Search: {Search}",
                userId, request.Page, request.Search);

            var query = _db.Recipes
                .Where(r => r.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keywords = request.Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(r => keywords.Any(k =>
                    r.Title.Contains(k) ||
                    r.Description.Contains(k) ||
                    r.Ingredients.Contains(k)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            _logger.LogInformation("Got {Count} of {Total} recipes for userId: {UserId}", items.Count, totalCount, userId);
            return (items, totalCount);
        }
        public async Task<Recipe?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting recipe by id: {id}", id);
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                _logger.LogWarning("Recipe with id {id} not found", id);
                return null;
            }
            _logger.LogInformation("Got recipe by id: {id}", id);
            return recipe;  
        }
        public async Task<Recipe>CreateAsync(Recipe recipe)
        {
            _logger.LogInformation("Creating recipe with title: {title}", recipe.Title);
            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created recipe with id: {id}", recipe.Id);
            return recipe;

        }
        public async Task<Recipe?> UpdateAsync(Recipe recipe)
        {
            _logger.LogInformation("Updating recipe with id: {id}", recipe.Id);
            _db.Recipes.Update(recipe);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated recipe with id: {id}", recipe.Id);
            return recipe;
        }
        public async Task<bool> DeleteAsync(int id,int userId)
        {
            _logger.LogInformation("Deleting recipe id: {Id} from db", id);
            var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (recipe is null) return false;
            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
