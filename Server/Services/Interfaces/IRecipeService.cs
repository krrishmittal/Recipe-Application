using Server.DTOs.Request;
using Server.DTOs.Response;

namespace Server.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<List<RecipeResponse>> GetAllAsync();
        Task<List<RecipeResponse>>GetMyRecipesAsync();
        Task<RecipeResponse> GetByIdAsync(int id);
        Task<RecipeResponse> CreateAsync(int userId, CreateRecipeRequest request);
        Task<RecipeResponse>UpdateAsync(int id, int userId, UpdateRecipeRequest request);
        Task<bool>DeleteAsync(int id, int userId);
    }
}
