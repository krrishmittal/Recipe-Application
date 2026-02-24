using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Request;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
public class RecipesController : BaseController
{
    private readonly IRecipeService _recipeService;
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(IRecipeService recipeService, ILogger<RecipesController> logger)
    {
        _recipeService = recipeService;
        _logger = logger;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Public - no auth needed
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching recipes page: ");
        var result = await _recipeService.GetAllAsync();
        return Success(result);
    }

    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetMyRecipes()
    {
        _logger.LogInformation("Fetching my recipes: ");
        var result = await _recipeService.GetMyRecipesAsync();
        if (result is null)
        {
            _logger.LogWarning("Recipe not found");
            return Fail("Recipe not found.", 404);
        }
        return Success(result);
    }

    //[AllowAnonymous]
    //[HttpGet("{id:int}")]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    _logger.LogInformation("Fetching recipe id: {Id}", id);
    //    var result = await _recipeService.GetByIdAsync(id);
    //    if (result is null)
    //    {
    //        _logger.LogWarning("Recipe not found id: {Id}", id);
    //        return Fail("Recipe not found.",404);
    //    }
    //    return Success(result);
    //}

    // Protected - auth required

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateRecipeRequest request)
    {
        _logger.LogInformation("Creating recipe for userId: {UserId}", GetUserId());
        var result = await _recipeService.CreateAsync(GetUserId(), request);
        return Created(result,nameof(_recipeService.GetByIdAsync), new { id = result.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateRecipeRequest request)
    {
        _logger.LogInformation("Updating recipe id: {Id}", id);
        var result = await _recipeService.UpdateAsync(id, GetUserId(), request);
        if (result is null)
        {
            _logger.LogWarning("Recipe not found or unauthorized id: {Id}", id);
            return Fail("Recipe not found or you are not logged in.",404);
        }
        return Success(result,"Recipe update successfully");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting recipe id: {Id}", id);
        var result = await _recipeService.DeleteAsync(id, GetUserId());
        if (!result)
        {
            _logger.LogWarning("Recipe not found or unauthorized id: {Id}", id);
            return Fail("Recipe not found or you are not logged in", 404);
        }
        return Success<Object?>(null, "Recipe deleted scuucessfully");
    }

}