using Microsoft.EntityFrameworkCore;
using Recipe.Infrastructure.Models;

namespace Recipe.Api.Services;

/// <summary>
/// Applies pending Entity Framework migrations during application startup.
/// </summary>
public static class MigrationManager
{
    /// <summary>
    /// Applies all pending database migrations.
    /// </summary>
    public static async Task ApplyMigrationsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(MigrationManager));

        logger.LogInformation("Applying pending database migrations.");
        await db.Database.MigrateAsync();
        logger.LogInformation("Database migrations completed.");
    }
}
