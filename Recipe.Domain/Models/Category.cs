using System;
using System.Collections.Generic;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a recipe category.
/// </summary>
public class Category
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipes in this category.
    /// </summary>
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
