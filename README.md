# Recipe Backend

This project is a layered ASP.NET Core Web API for a recipe platform with authentication, admin moderation, favorites, profile management, comments, ratings, categories, and tags.

## Projects

- `Recipe.Api`
  - API host, controllers, middleware, startup, Swagger, JWT auth, auto-migration
- `Recipe.Application`
  - commands, queries, validators, DTOs, shared response models
- `Recipe.Domain`
  - core entities and role constants
- `Recipe.Infrastructure`
  - EF Core `RecipeDbContext`, services, migrations, password hashing

## Main Features

- JWT authentication
- register, login, forgot password, reset password
- profile fetch and update
- change password
- delete account
- favorites
- recipe CRUD
- admin-only user and recipe moderation
- recipe categories and tags
- recipe comments
- recipe ratings
- publish/unpublish recipes
- feature/unfeature recipes

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- MediatR
- FluentValidation
- Serilog
- Cloudinary
- DotNetEnv

## Configuration

The API reads local secrets from `Recipe.Api/.env`.

Important keys:

```env
ConnectionStrings__DefaultConnection=

JwtSettings__SecretKey=
JwtSettings__Issuer=RecipeAPI
JwtSettings__Audience=RecipeAPIUsers
JwtSettings__ExpiryMinutes=10

EmailSettings__SenderEmail=
EmailSettings__SenderName=Recipe App
EmailSettings__Password=

OtpSettings__ExpiryMinutes=10

CloudinarySettings__CloudName=
CloudinarySettings__ApiKey=
CloudinarySettings__ApiSecret=

AdminSeed__Email=
AdminSeed__Password=
AdminSeed__Name=
```

## Startup Behavior

On startup the API:

1. loads `.env`
2. applies pending EF Core migrations automatically
3. checks the admin account rule
4. seeds the initial admin if no admin exists and `AdminSeed` values are configured

Admin policy:

- public registration always creates `User`
- no public admin registration route exists
- startup allows only one admin account
- the last admin cannot be deleted or demoted

## Run

From `Recipe.Api`:

```powershell
dotnet run --launch-profile https
```

Or from the repo root:

```powershell
dotnet run --project Recipe.Api\Recipe.Api.csproj --launch-profile https
```

Swagger is enabled in startup and opens on launch in development profiles.

## Data Notes

- IDs use `Guid`
- recipes are hard-deleted, not soft-deleted
- unpublished recipes are hidden from public recipe listing/detail/favorites
- `my-recipes` and admin flows can still see unpublished recipes

## Current Route Groups

- `api/auth`
- `api/recipes`
- `api/admin`

For the detailed API contract and response format, see [API_REFERENCE.md](./API_REFERENCE.md).
