namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents authentication data returned after registration or login.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets the JWT token.
    /// </summary>
    public string Token { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
