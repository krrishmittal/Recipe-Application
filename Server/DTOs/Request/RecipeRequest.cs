namespace Server.DTOs.Request;

public class CreateRecipeRequest 
{ 
    public string Title { get; set; } 
    public string Description { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string Ingredients { get; set; } = "[]";
    public string steps { get; set; } = "[]";
    public IFormFile Image { get; set; }
}

public class UpdateRecipeRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string Ingredients { get; set; } = "[]";
    public string steps { get; set; } = "[]";
    public IFormFile? Image { get; set; }
}
