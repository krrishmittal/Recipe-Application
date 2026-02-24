namespace Server.DTOs.Response;

public class RecipeResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string Ingredients { get; set; } = "[]";
    public string Steps { get; set; } = "[]";
    public string AuthorName { get; set; } = string.Empty;
    public int UserId { get; set; }
}