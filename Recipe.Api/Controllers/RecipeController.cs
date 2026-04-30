// RecipesController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Application.DTOs.Request;
using Recipe.Application.Features.Recipes;
using Recipe.Domain.Models;
using System.Security.Claims;

namespace Recipe.Api.Controllers;

/// <summary>
/// Exposes recipe endpoints for listing, reading, creating, updating, and deleting recipes.
/// </summary>
[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the RecipesController class.
    /// </summary>
    public RecipesController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool IsAdmin() =>
        User.IsInRole(UserRoles.Admin);

    /// <summary>
    /// Returns a paginated list of recipes with optional search.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _sender.Send(new GetAllRecipesQuery(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns a paginated list of recipes created by the authenticated user.
    /// </summary>
    [HttpGet("my-recipes")]
    public async Task<IActionResult> GetMyRecipes([FromQuery] PagedRequest request)
    {
        var result = await _sender.Send(new GetMyRecipesQuery(GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns the list of recipe categories.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _sender.Send(new GetRecipeCategoriesQuery());
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns the list of recipe tags.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        var result = await _sender.Send(new GetRecipeTagsQuery());
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns a paginated list of recipes saved by the authenticated user.
    /// </summary>
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites([FromQuery] PagedRequest request)
    {
        var result = await _sender.Send(new GetFavoriteRecipesQuery(GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns recipe details by identifier.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetRecipeByIdQuery(id));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Creates a new recipe for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateRecipeRequest request)
    {
        var result = await _sender.Send(new CreateRecipeCommand(GetUserId(), request));
        return StatusCode(result.Success ? 201 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates an existing recipe owned by the authenticated user.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateRecipeRequest request)
    {
        var result = await _sender.Send(new UpdateRecipeCommand(id, GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Deletes a recipe owned by the authenticated user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sender.Send(new DeleteRecipeCommand(id, GetUserId(), IsAdmin()));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Saves a recipe to the authenticated user's favorites.
    /// </summary>
    [HttpPost("{id:guid}/favorite")]
    public async Task<IActionResult> AddFavorite(Guid id)
    {
        var result = await _sender.Send(new AddFavoriteCommand(GetUserId(), id));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Removes a recipe from the authenticated user's favorites.
    /// </summary>
    [HttpDelete("{id:guid}/favorite")]
    public async Task<IActionResult> RemoveFavorite(Guid id)
    {
        var result = await _sender.Send(new RemoveFavoriteCommand(GetUserId(), id));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Adds a comment to a recipe.
    /// </summary>
    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddRecipeCommentRequest request)
    {
        var result = await _sender.Send(new AddRecipeCommentCommand(id, GetUserId(), request.Content));
        return StatusCode(result.Success ? 201 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates a recipe comment.
    /// </summary>
    [HttpPut("comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateRecipeCommentRequest request)
    {
        var result = await _sender.Send(new UpdateRecipeCommentCommand(commentId, GetUserId(), IsAdmin(), request.Content));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Deletes a recipe comment.
    /// </summary>
    [HttpDelete("comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var result = await _sender.Send(new DeleteRecipeCommentCommand(commentId, GetUserId(), IsAdmin()));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Adds or updates the current user's rating for a recipe.
    /// </summary>
    [HttpPost("{id:guid}/ratings")]
    public async Task<IActionResult> RateRecipe(Guid id, [FromBody] RateRecipeRequest request)
    {
        var result = await _sender.Send(new RateRecipeCommand(id, GetUserId(), request.Value));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Removes the current user's rating for a recipe.
    /// </summary>
    [HttpDelete("{id:guid}/ratings")]
    public async Task<IActionResult> DeleteRating(Guid id)
    {
        var result = await _sender.Send(new DeleteRecipeRatingCommand(id, GetUserId()));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }
}
