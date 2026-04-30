using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Services.Interfaces;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Handles the register command.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResponse>>
{
    private readonly RecipeDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<RegisterCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the RegisterCommandHandler class.
    /// </summary>
    public RegisterCommandHandler(
        RecipeDbContext db,
        IJwtService jwtService,
        IEmailService emailService,
        IConfiguration config,
        ILogger<RegisterCommandHandler> logger)
    {
        _db = db;
        _jwtService = jwtService;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Handles the MediatR request.
    /// </summary>
    public async Task<ApiResponse<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Register attempt for {Email}", request.Request.Email);

            var email = request.Request.Email.ToLowerInvariant();
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (existing is not null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Request.Email);
                return ApiResponse<AuthResponse>.Fail("Email already registered.", 409, nameof(RegisterCommand));
            }

            var user = new User
            {
                Name = request.Request.Name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);

            var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]!));
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name, user.Role);

            return ApiResponse<AuthResponse>.Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                ExpiresAt = expiry
            }, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Handler}", nameof(RegisterCommandHandler));
            return ApiResponse<AuthResponse>.Fail("Registration failed due to an unexpected error.", 500, nameof(RegisterCommand));
        }
    }
}
