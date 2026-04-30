using System;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a tag attached to a recipe.
/// </summary>
public class RecipeTag
{
    /// <summary>
    /// Gets or sets the recipe identifier.
    /// </summary>
    public Guid RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the tag identifier.
    /// </summary>
    public Guid TagId { get; set; }

    /// <summary>
    /// Gets or sets the tagged recipe.
    /// </summary>
    public Recipe Recipe { get; set; } = null!;

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    public Tag Tag { get; set; } = null!;
}
