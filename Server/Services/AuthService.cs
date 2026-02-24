using Microsoft.EntityFrameworkCore;
using Server.DTOs.Request;
using Server.DTOs.Response;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.Services;

public class AuthService : IAuthService
{
    private readonly RecipeDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(RecipeDbContext db, IJwtService jwtService, IOtpService otpService, IEmailService emailService, IConfiguration config, ILogger<AuthService> logger)
    {
        _db = db; _jwtService = jwtService; _otpService = otpService; _emailService = emailService; _config = config; _logger = logger;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Register attempt for {Email}", request.Email);

        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (existing is not null)
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
            return null;
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
        _logger.LogInformation("User registered successfully: {Email}", request.Email);

        var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]!));
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            ExpiresAt = expiry
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null)
        {
            _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed - wrong password: {Email}", request.Email);
            return null;
        }

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);

        var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]!));
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

        return new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            ExpiresAt = expiry
        };
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        _logger.LogInformation("Forgot password request for {Email}", request.Email);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null)
        {
            // Return true anyway to avoid email enumeration
            _logger.LogWarning("Forgot password - user not found: {Email}", request.Email);
            return true;
        }

        var otpCode = await _otpService.GenerateAndStoreOtpAsync(user.Id);
        await _emailService.SendOtpEmailAsync(user.Email, user.Name, otpCode);

        _logger.LogInformation("OTP sent to {Email}", request.Email);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        _logger.LogInformation("Reset password attempt for {Email}", request.Email);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null)
        {
            _logger.LogWarning("Reset password - user not found: {Email}", request.Email);
            return false;
        }

        var isValid = await _otpService.ValidateOtpAsync(user.Id, request.OtpCode);
        if (!isValid)
        {
            _logger.LogWarning("Reset password - invalid OTP for {Email}", request.Email);
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Password reset successfully for {Email}", request.Email);
        return true;
    }
}