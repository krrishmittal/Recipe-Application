using System;
using System.Collections.Generic;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a reusable recipe tag.
/// </summary>
public class Tag
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipes that use this tag.
    /// </summary>
    public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
}
