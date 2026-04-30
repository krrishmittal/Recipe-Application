using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Requests the list of recipe tags.
/// </summary>
public record GetRecipeTagsQuery() : IRequest<ApiResponse<List<string>>>;
