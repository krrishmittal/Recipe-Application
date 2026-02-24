using Server.Models;

namespace Server.Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAllAsync();
        Task<List<Recipe>> GetMyRecipesAsync(int id);
        Task<Recipe> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task<Recipe> UpdateAsync(Recipe recipe);
        Task<bool> DeleteAsync(int id,int userId);

    }
}
