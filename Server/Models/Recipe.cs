using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int PrepTimeMinutes { get; set; }

    public int CookTimeMinutes { get; set; }

    public string Ingredients { get; set; } = null!;

    public string Steps { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
