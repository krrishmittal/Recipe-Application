using System;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a recipe saved as a favorite by a user.
/// </summary>
public partial class FavoriteRecipe
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the recipe identifier.
    /// </summary>
    public Guid RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the owning user.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the saved recipe.
    /// </summary>
    public virtual Recipe Recipe { get; set; } = null!;
}
