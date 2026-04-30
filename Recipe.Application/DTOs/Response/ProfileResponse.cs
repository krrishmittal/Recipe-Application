namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents profile data returned for the current user.
/// </summary>
public class UserProfileResponse
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile bio.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the profile image URL.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of recipes created by the user.
    /// </summary>
    public int RecipeCount { get; set; }

    /// <summary>
    /// Gets or sets the number of favorite recipes saved by the user.
    /// </summary>
    public int FavoriteCount { get; set; }
}
