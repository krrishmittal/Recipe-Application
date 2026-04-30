using System;
using System.Collections.Generic;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents a recipe owned by a user.
/// </summary>
public partial class Recipe
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the recipe title.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the recipe description.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hosted image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the preparation time in minutes.
    /// </summary>
    public int PrepTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the cooking time in minutes.
    /// </summary>
    public int CookTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the recipe ingredients.
    /// </summary>
    public string Ingredients { get; set; } = null!;

    /// <summary>
    /// Gets or sets the recipe steps.
    /// </summary>
    public string Steps { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets whether the recipe is published.
    /// </summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the recipe is featured.
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the owning user.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the users who saved this recipe.
    /// </summary>
    public virtual ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();

    /// <summary>
    /// Gets or sets the tags attached to the recipe.
    /// </summary>
    public virtual ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();

    /// <summary>
    /// Gets or sets the comments on the recipe.
    /// </summary>
    public virtual ICollection<RecipeComment> RecipeComments { get; set; } = new List<RecipeComment>();

    /// <summary>
    /// Gets or sets the ratings on the recipe.
    /// </summary>
    public virtual ICollection<RecipeRating> RecipeRatings { get; set; } = new List<RecipeRating>();
}
