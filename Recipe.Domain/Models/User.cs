using System;
using System.Collections.Generic;

namespace Recipe.Domain.Models;

/// <summary>
/// Represents an application user.
/// </summary>
public partial class User
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = UserRoles.User;

    /// <summary>
    /// Gets or sets the profile bio.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the profile image URL.
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the recipes owned by the user.
    /// </summary>
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    /// <summary>
    /// Gets or sets the recipes saved by the user.
    /// </summary>
    public virtual ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();

    /// <summary>
    /// Gets or sets the comments authored by the user.
    /// </summary>
    public virtual ICollection<RecipeComment> RecipeComments { get; set; } = new List<RecipeComment>();

    /// <summary>
    /// Gets or sets the ratings authored by the user.
    /// </summary>
    public virtual ICollection<RecipeRating> RecipeRatings { get; set; } = new List<RecipeRating>();
}

/// <summary>
/// Defines supported application roles.
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// The standard user role.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// The administrator role.
    /// </summary>
    public const string Admin = "Admin";
}
