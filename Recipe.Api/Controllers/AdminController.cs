using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Application.DTOs.Request;
using Recipe.Application.Features.Admin;
using Recipe.Application.Features.Recipes;
using Recipe.Domain.Models;

namespace Recipe.Api.Controllers;

/// <summary>
/// Exposes admin-only endpoints for user management and recipe moderation.
/// </summary>
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the AdminController class.
    /// </summary>
    public AdminController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Returns a paginated list of users for the admin area.
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] PagedRequest request)
    {
        var result = await _sender.Send(new GetAllUsersQuery(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates the role of a target user.
    /// </summary>
    [HttpPatch("users/{id:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        var result = await _sender.Send(new UpdateUserRoleCommand(id, request.Role));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Permanently deletes a target user.
    /// </summary>
    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _sender.Send(new DeleteUserCommand(id));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns a paginated list of recipes for the admin area.
    /// </summary>
    [HttpGet("recipes")]
    public async Task<IActionResult> GetRecipes([FromQuery] PagedRequest request)
    {
        var result = await _sender.Send(new GetAllRecipesForAdminQuery(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates whether a recipe is published.
    /// </summary>
    [HttpPatch("recipes/{id:guid}/publish")]
    public async Task<IActionResult> UpdatePublishStatus(Guid id, [FromBody] UpdateRecipePublishStatusRequest request)
    {
        var result = await _sender.Send(new UpdateRecipePublishStatusCommand(id, request.IsPublished));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates whether a recipe is featured.
    /// </summary>
    [HttpPatch("recipes/{id:guid}/feature")]
    public async Task<IActionResult> UpdateFeaturedStatus(Guid id, [FromBody] UpdateRecipeFeaturedStatusRequest request)
    {
        var result = await _sender.Send(new UpdateRecipeFeaturedStatusCommand(id, request.IsFeatured));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Permanently deletes any recipe as an admin.
    /// </summary>
    [HttpDelete("recipes/{id:guid}")]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        var result = await _sender.Send(new DeleteRecipeCommand(id, Guid.Empty, true));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }
}
