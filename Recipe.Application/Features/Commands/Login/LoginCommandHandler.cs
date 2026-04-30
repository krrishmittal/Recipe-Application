using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the login command.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the LoginCommandHandler class.
    /// </summary>
    public LoginCommandHandler(
        RecipeDbContext db,
        IJwtService jwtService,
        IConfiguration config,
        ILogger<LoginCommandHandler> logger)
    {
        _db = db;
        _jwtService = jwtService;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Login attempt for {Email}", request.Request.Email);

            var email = request.Request.Email.ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for {Email}", request.Request.Email);
                return ApiResponse<AuthResponse>.Fail("Invalid email or password.", 401, nameof(LoginCommand));
            }

            var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]!));
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name, user.Role);

            return ApiResponse<AuthResponse>.Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                ExpiresAt = expiry
            }, "Login successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(LoginCommandHandler));
            return ApiResponse<AuthResponse>.Fail("Login failed due to an unexpected error.", 500, nameof(LoginCommand));
        }
    }
}
