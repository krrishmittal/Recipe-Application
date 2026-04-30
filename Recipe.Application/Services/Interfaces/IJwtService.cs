namespace Recipe.Application.Services.Interfaces;

/// <summary>
/// Defines JWT token generation operations.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a signed JWT token for the specified user.
    /// </summary>
    string GenerateToken(Guid userId, string email, string name, string role);
}
