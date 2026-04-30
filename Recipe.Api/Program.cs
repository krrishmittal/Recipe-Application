using CloudinaryDotNet;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Recipe.Application.DTOs.Response;
using Recipe.Application.Features;
using Recipe.Application.Features.Common.Behaviors;
using Recipe.Api.Middleware;
using Recipe.Api.Services;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Models;
using Recipe.Infrastructure.Services;
using Recipe.Application.Services.Interfaces;
using Recipe.Infrastructure.Handlers;
using System.Text;
using System.Text.Json;

Env.Load();
var builder = WebApplication.CreateBuilder(args);



builder.Configuration.AddJsonFile("serilogsettings.json", optional: true);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();



builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var endpoint = context.HttpContext.GetEndpoint();
                var methodName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ActionName
                                 ?? "Unknown";

                var response = ApiResponse<object>.Fail(
                    "Unauthorized.",
                    401,
                    methodName
                );

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var endpoint = context.HttpContext.GetEndpoint();
                var methodName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ActionName
                                 ?? "Unknown";

                var response = ApiResponse<object>.Fail(
                    "Forbidden.",
                    403,
                    methodName
                );

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Recipe API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();
builder.Services.AddMediatR(
    typeof(AssemblyMarker).Assembly,
    typeof(JwtService).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


try
{
    Log.Information("Starting web host");
    var app = builder.Build();
    await MigrationManager.ApplyMigrationsAsync(app.Services);
    await EnsureAdminAccountAsync(app.Services, app.Configuration);

    //if (app.Environment.IsDevelopment())
    //{
        app.UseSwagger();
        app.UseSwaggerUI();
    //}

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task EnsureAdminAccountAsync(IServiceProvider services, IConfiguration configuration)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeed");

    var adminCount = await db.Users.CountAsync(u => u.Role == UserRoles.Admin);
    if (adminCount > 1)
    {
        throw new InvalidOperationException("Only one admin account is allowed. Clean up the database so a single admin remains.");
    }

    if (adminCount == 1)
    {
        return;
    }

    var adminSeed = configuration.GetSection("AdminSeed");
    var email = adminSeed["Email"]?.Trim().ToLowerInvariant();
    var password = adminSeed["Password"]?.Trim();
    var name = adminSeed["Name"]?.Trim();

    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password) ||
        string.IsNullOrWhiteSpace(name))
    {
        throw new InvalidOperationException(
            "No admin account exists. Configure AdminSeed:Email, AdminSeed:Password, and AdminSeed:Name before starting the API.");
    }

    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (existingUser is null)
    {
        existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = PasswordHashing.HashP(password),
            CreatedAt = DateTime.UtcNow,
            Role = UserRoles.Admin
        };

        db.Users.Add(existingUser);
        logger.LogInformation("Seeded initial admin account for {Email}", email);
    }
    else
    {
        existingUser.Name = name;
        existingUser.PasswordHash = PasswordHashing.HashP(password);
        existingUser.Role = UserRoles.Admin;
        logger.LogInformation("Promoted configured admin account for {Email}", email);
    }

    await db.SaveChangesAsync();
}
