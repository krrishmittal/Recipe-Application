using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Request;

using Server.Services.Interfaces;

namespace Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Register attempt for {Email}", request.Email);
        var result = await _authService.RegisterAsync(request);
        if (result is null)
        {
            _logger.LogWarning("Registration failed for {Email}", request.Email);
            return Fail("Email already registered.", 409);
        }
        _logger.LogInformation("Registration successful for {Email}", request.Email);
        return Success(result, "Registeration Successful.");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);
        var result = await _authService.LoginAsync(request);
        if (result is null)
        {
            _logger.LogWarning("Login failed for {Email}", request.Email);
            return Fail("Invalid email or password.", 401);
        }
        _logger.LogInformation("Login successful for {Email}", request.Email);
        return Success(result,"Login successful.");
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation("Forgot password request for {Email}", request.Email);
        await _authService.ForgotPasswordAsync(request);
        // Always return 200 to avoid email enumeration
        return Success<Object?>(null,"If the email exists, an OTP has been sent.");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        _logger.LogInformation("Reset password attempt for {Email}", request.Email);
        var result = await _authService.ResetPasswordAsync(request);
        if (!result)
        {
            _logger.LogWarning("Reset password failed for {Email}", request.Email);
            return Fail("Invalid or expired OTP.", 400);
        }
        _logger.LogInformation("Password reset successful for {Email}", request.Email);
        return Success<Object?>(null,"Password reset successful.");
    }
}