using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Requests a role update for a user.
/// </summary>
public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<ApiResponse<UserProfileResponse>>;
