using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe.Infrastructure.Migrations
{
    /// <summary>
    /// Adds profile fields, recipe visibility flags, and favorites support.
    /// </summary>
    public partial class AddAccountsProfilesAndFavorites : Migration
    {
        /// <summary>
        /// Applies the schema changes.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns c
                    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID(N'[Users]')
                      AND c.name = N'Id'
                      AND t.name = N'int'
                )
                BEGIN
                    IF OBJECT_ID(N'[FK_OtpRecords_Users_UserId]', N'F') IS NOT NULL
                        ALTER TABLE [OtpRecords] DROP CONSTRAINT [FK_OtpRecords_Users_UserId];

                    IF OBJECT_ID(N'[FK_Recipes_Users_UserId]', N'F') IS NOT NULL
                        ALTER TABLE [Recipes] DROP CONSTRAINT [FK_Recipes_Users_UserId];

                    IF EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE object_id = OBJECT_ID(N'[Recipes]')
                          AND name = N'IX_Recipes_UserId'
                    )
                        DROP INDEX [IX_Recipes_UserId] ON [Recipes];

                    ALTER TABLE [Users] ADD [NewId] uniqueidentifier NULL;
                    ALTER TABLE [Recipes] ADD [NewId] uniqueidentifier NULL;
                    ALTER TABLE [Recipes] ADD [NewUserId] uniqueidentifier NULL;
                    ALTER TABLE [OtpRecords] ADD [NewUserId] uniqueidentifier NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'[Users]', N'NewId') IS NOT NULL
                BEGIN
                    UPDATE [Users] SET [NewId] = NEWID() WHERE [NewId] IS NULL;
                    ALTER TABLE [Users] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'[Recipes]', N'NewId') IS NOT NULL
                BEGIN
                    UPDATE [Recipes] SET [NewId] = NEWID() WHERE [NewId] IS NULL;
                    UPDATE r
                    SET [NewUserId] = u.[NewId]
                    FROM [Recipes] AS r
                    INNER JOIN [Users] AS u ON r.[UserId] = u.[Id];

                    ALTER TABLE [Recipes] ALTER COLUMN [NewId] uniqueidentifier NOT NULL;
                    ALTER TABLE [Recipes] ALTER COLUMN [NewUserId] uniqueidentifier NOT NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'[OtpRecords]', N'NewUserId') IS NOT NULL
                BEGIN
                    UPDATE o
                    SET [NewUserId] = u.[NewId]
                    FROM [OtpRecords] AS o
                    INNER JOIN [Users] AS u ON o.[UserId] = u.[Id];

                    ALTER TABLE [OtpRecords] ALTER COLUMN [NewUserId] uniqueidentifier NOT NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH(N'[Users]', N'NewId') IS NOT NULL
                BEGIN
                    ALTER TABLE [OtpRecords] DROP CONSTRAINT [PK_OtpRecords];
                    ALTER TABLE [Recipes] DROP CONSTRAINT [PK_Recipes];
                    ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];

                    ALTER TABLE [OtpRecords] DROP COLUMN [UserId];
                    ALTER TABLE [Recipes] DROP COLUMN [Id];
                    ALTER TABLE [Recipes] DROP COLUMN [UserId];
                    ALTER TABLE [Users] DROP COLUMN [Id];

                    EXEC sp_rename N'[Users].[NewId]', N'Id', N'COLUMN';
                    EXEC sp_rename N'[Recipes].[NewId]', N'Id', N'COLUMN';
                    EXEC sp_rename N'[Recipes].[NewUserId]', N'UserId', N'COLUMN';
                    EXEC sp_rename N'[OtpRecords].[NewUserId]', N'UserId', N'COLUMN';

                    ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY ([Id]);
                    ALTER TABLE [Recipes] ADD CONSTRAINT [PK_Recipes] PRIMARY KEY ([Id]);
                    ALTER TABLE [OtpRecords] ADD CONSTRAINT [PK_OtpRecords] PRIMARY KEY ([UserId]);

                    CREATE INDEX [IX_Recipes_UserId] ON [Recipes] ([UserId]);

                    ALTER TABLE [OtpRecords]
                        ADD CONSTRAINT [FK_OtpRecords_Users_UserId]
                        FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;

                    ALTER TABLE [Recipes]
                        ADD CONSTRAINT [FK_Recipes_Users_UserId]
                        FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
                END
                """);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "User");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "FavoriteRecipes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteRecipes", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_FavoriteRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRecipes_RecipeId",
                table: "FavoriteRecipes",
                column: "RecipeId");
        }

        /// <summary>
        /// Reverts the schema changes.
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteRecipes");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Recipes");
        }
    }
}
