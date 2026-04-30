using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the query used to get my profile.
/// </summary>
public record GetMyProfileQuery(Guid UserId) : IRequest<ApiResponse<UserProfileResponse>>;
