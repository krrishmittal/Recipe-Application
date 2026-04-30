// AuthController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Application.DTOs.Request;
using Recipe.Application.Features.Auth;
using System.Security.Claims;

namespace Recipe.Api.Controllers;

/// <summary>
/// Exposes authentication endpoints for account registration and sign-in flows.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// </summary>
    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _sender.Send(new RegisterCommand(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _sender.Send(new LoginCommand(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Starts the forgot-password flow by sending an OTP if the account exists.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _sender.Send(new ForgotPasswordCommand(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Resets a user password using OTP verification.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _sender.Send(new ResetPasswordCommand(request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Returns the current user's profile.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _sender.Send(new GetMyProfileQuery(GetUserId()));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Updates the current user's profile.
    /// </summary>
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
    {
        var result = await _sender.Send(new UpdateProfileCommand(GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _sender.Send(new ChangePasswordCommand(GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }

    /// <summary>
    /// Deletes the current user's account permanently.
    /// </summary>
    [Authorize]
    [HttpPost("delete-account")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        var result = await _sender.Send(new DeleteAccountCommand(GetUserId(), request));
        return StatusCode(result.Success ? 200 : (result.Errors?.FirstOrDefault()?.Code ?? 500), result);
    }
}
