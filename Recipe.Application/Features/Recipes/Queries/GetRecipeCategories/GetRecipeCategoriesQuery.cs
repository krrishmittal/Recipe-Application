using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests the list of recipe categories.
/// </summary>
public record GetRecipeCategoriesQuery() : IRequest<ApiResponse<List<string>>>;
