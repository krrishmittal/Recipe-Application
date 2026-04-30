using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Recipe.Domain.Models;
using RecipeEntity = Recipe.Domain.Models.Recipe;

namespace Recipe.Infrastructure.Models;

/// <summary>
/// Entity Framework database context for recipe application data.
/// </summary>
public partial class RecipeDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the RecipeDbContext class.
    /// </summary>
    public RecipeDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RecipeDbContext class.
    /// </summary>
    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets OTP records.
    /// </summary>
    public virtual DbSet<OtpRecord> OtpRecords { get; set; }

    /// <summary>
    /// Gets or sets favorite recipes.
    /// </summary>
    public virtual DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }

    /// <summary>
    /// Gets or sets categories.
    /// </summary>
    public virtual DbSet<Category> Categories { get; set; }

    /// <summary>
    /// Gets or sets tags.
    /// </summary>
    public virtual DbSet<Tag> Tags { get; set; }

    /// <summary>
    /// Gets or sets recipe tags.
    /// </summary>
    public virtual DbSet<RecipeTag> RecipeTags { get; set; }

    /// <summary>
    /// Gets or sets recipe comments.
    /// </summary>
    public virtual DbSet<RecipeComment> RecipeComments { get; set; }

    /// <summary>
    /// Gets or sets recipe ratings.
    /// </summary>
    public virtual DbSet<RecipeRating> RecipeRatings { get; set; }

    /// <summary>
    /// Gets or sets recipes.
    /// </summary>
    public virtual DbSet<RecipeEntity> Recipes { get; set; }

    /// <summary>
    /// Gets or sets users.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>
    /// Configures the database context options.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    /// <summary>
    /// Configures entity mappings and relationships.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteRecipe>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RecipeId });
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.RecipeId).ValueGeneratedNever();

            entity.HasOne(d => d.User)
                .WithMany(p => p.FavoriteRecipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.Recipe)
                .WithMany(p => p.FavoriteRecipes)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<RecipeTag>(entity =>
        {
            entity.HasKey(e => new { e.RecipeId, e.TagId });
            entity.Property(e => e.RecipeId).ValueGeneratedNever();
            entity.Property(e => e.TagId).ValueGeneratedNever();

            entity.HasOne(d => d.Recipe)
                .WithMany(p => p.RecipeTags)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Tag)
                .WithMany(p => p.RecipeTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeComment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.HasIndex(e => e.RecipeId);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(d => d.Recipe)
                .WithMany(p => p.RecipeComments)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.RecipeComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<RecipeRating>(entity =>
        {
            entity.HasKey(e => new { e.RecipeId, e.UserId });
            entity.Property(e => e.RecipeId).ValueGeneratedNever();
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.HasIndex(e => e.UserId);

            entity.HasOne(d => d.Recipe)
                .WithMany(p => p.RecipeRatings)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.RecipeRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<OtpRecord>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<OtpRecord>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeEntity>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Recipes_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.IsPublished).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.HasIndex(e => e.CategoryId);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Recipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Recipes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(1000);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
