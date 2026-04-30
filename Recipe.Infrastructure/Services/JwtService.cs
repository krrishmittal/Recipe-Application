using Recipe.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Recipe.Infrastructure.Services;
/// <summary>
/// Generates JWT tokens for authenticated users.
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    /// <summary>
    /// Initializes a new instance of the JwtService class.
    /// </summary>
    public JwtService(IConfiguration config)
    {
        _config = config;
    }
    /// <summary>
    /// Generates a signed JWT token for the specified user.
    /// </summary>
    public string GenerateToken(Guid userId, string email, string name, string role)
    {
        var settings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var expiry=DateTime.UtcNow.AddMinutes(double.Parse(settings["ExpiryMinutes"]!));
        var claims= new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: settings["Issuer"],
            audience: settings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
