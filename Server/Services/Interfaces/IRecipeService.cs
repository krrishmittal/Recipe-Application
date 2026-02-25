using Server.DTOs.Request;
using Server.DTOs.Response;

namespace Server.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<ApiResponse<PagedResponse<RecipeResponse>>> GetAllAsync(PagedRequest request);
        Task<ApiResponse<PagedResponse<RecipeResponse>>> GetMyRecipesAsync(PagedRequest request);
        Task<ApiResponse<RecipeResponse>> GetByIdAsync(int id);
        Task<ApiResponse<RecipeResponse>> CreateAsync(int userId, CreateRecipeRequest request);
        Task<ApiResponse<RecipeResponse>> UpdateAsync(int id, int userId, UpdateRecipeRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}
