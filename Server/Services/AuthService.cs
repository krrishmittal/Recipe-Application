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

    public AuthService(RecipeDbContext db, IJwtService jwtService, IOtpService otpService,
        IEmailService emailService, IConfiguration config, ILogger<AuthService> logger)
    {
        _db = db;
        _jwtService = jwtService;
        _otpService = otpService;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Register attempt for {Email}", request.Email);

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (existing is not null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
                return ApiResponse<AuthResponse>.Fail("Email already registered.", 409, nameof(RegisterAsync));
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

            return ApiResponse<AuthResponse>.Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                ExpiresAt = expiry
            }, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(RegisterAsync));
            return ApiResponse<AuthResponse>.Fail("Something went wrong.", 500, nameof(RegisterAsync));
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for {Email}", request.Email);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for {Email}", request.Email);
                return ApiResponse<AuthResponse>.Fail("Invalid email or password.", 401, nameof(LoginAsync));
            }

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiryMinutes"]!));
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Name);

            return ApiResponse<AuthResponse>.Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                ExpiresAt = expiry
            }, "Login successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(LoginAsync));
            return ApiResponse<AuthResponse>.Fail("Something went wrong.", 500, nameof(LoginAsync));
        }
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            _logger.LogInformation("Forgot password request for {Email}", request.Email);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (user is not null)
            {
                var otpCode = await _otpService.GenerateAndStoreOtpAsync(user.Id);
                await _emailService.SendOtpEmailAsync(user.Email, user.Name, otpCode);
                _logger.LogInformation("OTP sent to {Email}", request.Email);
            }

            return ApiResponse<bool>.Ok(true, "If the email exists, an OTP has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(ForgotPasswordAsync));
            return ApiResponse<bool>.Fail("Something went wrong.", 500, nameof(ForgotPasswordAsync));
        }
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            _logger.LogInformation("Reset password attempt for {Email}", request.Email);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
            if (user is null)
            {
                _logger.LogWarning("Reset password - user not found: {Email}", request.Email);
                return ApiResponse<bool>.Fail("Invalid or expired OTP.", 400, nameof(ResetPasswordAsync));
            }

            var isValid = await _otpService.ValidateOtpAsync(user.Id, request.OtpCode);
            if (!isValid)
            {
                _logger.LogWarning("Reset password - invalid OTP for {Email}", request.Email);
                return ApiResponse<bool>.Fail("Invalid or expired OTP.", 400, nameof(ResetPasswordAsync));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Password reset successfully for {Email}", request.Email);
            return ApiResponse<bool>.Ok(true, "Password reset successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method}", nameof(ResetPasswordAsync));
            return ApiResponse<bool>.Fail("Something went wrong.", 500, nameof(ResetPasswordAsync));
        }
    }
}