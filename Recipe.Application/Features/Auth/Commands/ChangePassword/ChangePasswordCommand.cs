using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to change password.
/// </summary>
public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : IRequest<ApiResponse<bool>>;
