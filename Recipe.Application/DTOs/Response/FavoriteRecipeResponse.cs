namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents a recipe saved in the current user's favorites.
/// </summary>
public class FavoriteRecipeResponse
{
    /// <summary>
    /// Gets or sets the recipe identifier.
    /// </summary>
    public Guid RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the recipe title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipe description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hosted image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the recipe author name.
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date and time for when the favorite was added.
    /// </summary>
    public DateTime FavoritedAt { get; set; }
}
