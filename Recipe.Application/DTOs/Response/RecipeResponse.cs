namespace Recipe.Application.DTOs.Response;

/// <summary>
/// Represents recipe data returned to clients.
/// </summary>
public class RecipeResponse
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }
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
    public string Steps { get; set; } = "[]";
    /// <summary>
    /// Gets or sets the recipe author name.
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets whether the recipe is published.
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Gets or sets whether the recipe is featured.
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the recipe tags.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the average rating.
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Gets or sets the rating count.
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Gets or sets the latest comments.
    /// </summary>
    public List<RecipeCommentResponse> Comments { get; set; } = new();
}

/// <summary>
/// Represents recipe comment data returned to clients.
/// </summary>
public class RecipeCommentResponse
{
    /// <summary>
    /// Gets or sets the comment identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the author identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comment content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the comment was updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Represents recipe rating summary data returned to clients.
/// </summary>
public class RecipeRatingResponse
{
    /// <summary>
    /// Gets or sets the recipe identifier.
    /// </summary>
    public Guid RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the current user's rating.
    /// </summary>
    public int? UserRating { get; set; }

    /// <summary>
    /// Gets or sets the average rating.
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Gets or sets the rating count.
    /// </summary>
    public int RatingCount { get; set; }
}
