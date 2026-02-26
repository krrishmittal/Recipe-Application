// RecipesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Request;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers;

[ApiController]
[Route("api/recipes")]
//[Authorize]
public class RecipesController : ControllerBase
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
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _recipeService.GetAllAsync(request);
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetMyRecipes([FromQuery] PagedRequest request)
    {
        var result = await _recipeService.GetMyRecipesAsync(request);
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _recipeService.GetByIdAsync(id);
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateRecipeRequest request)
    {
        var result = await _recipeService.CreateAsync(GetUserId(), request);
        return StatusCode(result.Success ? 201 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateRecipeRequest request)
    {
        var result = await _recipeService.UpdateAsync(id, GetUserId(), request);
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _recipeService.DeleteAsync(id, GetUserId());
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }
}