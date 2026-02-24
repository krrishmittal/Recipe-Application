using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Recipe>> GetAllAsync()
        {
            _logger.LogInformation("Getting all recipes");
            var recipes = await _db.Recipes.ToListAsync();
            _logger.LogInformation("Got all recipes");
            return recipes;
            
        }
        public async Task<List<Recipe>>GetMyRecipesAsync(int userId)
        {
            _logger.LogInformation("Getting recipes for user id: {userId}", userId);
            var recipes = await _db.Recipes.Where(r => r.UserId == userId).ToListAsync();
            _logger.LogInformation("Got recipes for user id: {userId}", userId);
            return recipes;
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
            var existingRecipe = await _db.Recipes.FindAsync(recipe.Id);
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
