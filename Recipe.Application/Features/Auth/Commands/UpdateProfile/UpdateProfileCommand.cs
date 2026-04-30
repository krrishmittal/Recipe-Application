using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to update profile.
/// </summary>
public record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Request) : IRequest<ApiResponse<UserProfileResponse>>;
