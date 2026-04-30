using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Requests a paginated list of users for the admin area.
/// </summary>
public record GetAllUsersQuery(PagedRequest Request) : IRequest<ApiResponse<PagedResponse<UserProfileResponse>>>;
