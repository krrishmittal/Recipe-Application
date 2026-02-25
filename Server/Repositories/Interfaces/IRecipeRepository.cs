using Server.DTOs.Request;
using Server.Models;

namespace Server.Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        Task<(List<Recipe> Items, int TotalCount)> GetAllAsync(PagedRequest request);
        Task<(List<Recipe> Items, int TotalCount)> GetMyRecipesAsync(int userId, PagedRequest request);
        Task<Recipe> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task<Recipe> UpdateAsync(Recipe recipe);
        Task<bool> DeleteAsync(int id,int userId);

    }
}
