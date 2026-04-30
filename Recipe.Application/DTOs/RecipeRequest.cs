namespace Recipe.Application.DTOs.Request;

/// <summary>
/// Represents the form-data payload used to create a recipe.
/// </summary>
public class CreateRecipeRequest 
{ 
    /// <summary>
    /// Gets or sets the recipe title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the recipe description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
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
    public string Ingredients { get; set; } = "[]";
    /// <summary>
    /// Gets or sets the recipe steps.
    /// </summary>
    public string steps { get; set; } = "[]";
    /// <summary>
    /// Gets or sets the uploaded recipe image.
    /// </summary>
    public IFormFile Image { get; set; } = default!;

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated tags.
    /// </summary>
    public string? Tags { get; set; }
}

/// <summary>
/// Represents the form-data payload used to update a recipe.
/// </summary>
public class UpdateRecipeRequest
{
    /// <summary>
    /// Gets or sets the recipe title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the recipe description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
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
    public string Ingredients { get; set; } = "[]";
    /// <summary>
    /// Gets or sets the recipe steps.
    /// </summary>
    public string steps { get; set; } = "[]";
    /// <summary>
    /// Gets or sets the uploaded recipe image.
    /// </summary>
    public IFormFile? Image { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated tags.
    /// </summary>
    public string? Tags { get; set; }
}

/// <summary>
/// Represents the request payload used by admins to publish or unpublish a recipe.
/// </summary>
public class UpdateRecipePublishStatusRequest
{
    /// <summary>
    /// Gets or sets whether the recipe should be published.
    /// </summary>
    public bool IsPublished { get; set; }
}

/// <summary>
/// Represents the request payload used by admins to feature or unfeature a recipe.
/// </summary>
public class UpdateRecipeFeaturedStatusRequest
{
    /// <summary>
    /// Gets or sets whether the recipe should be featured.
    /// </summary>
    public bool IsFeatured { get; set; }
}

/// <summary>
/// Represents the request payload used to add a comment to a recipe.
/// </summary>
public class AddRecipeCommentRequest
{
    /// <summary>
    /// Gets or sets the comment content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to update a recipe comment.
/// </summary>
public class UpdateRecipeCommentRequest
{
    /// <summary>
    /// Gets or sets the updated comment content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Represents the request payload used to add or update a recipe rating.
/// </summary>
public class RateRecipeRequest
{
    /// <summary>
    /// Gets or sets the rating value.
    /// </summary>
    public int Value { get; set; }
}
