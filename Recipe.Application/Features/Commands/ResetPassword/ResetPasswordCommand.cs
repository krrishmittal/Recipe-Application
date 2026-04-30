using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to reset password.
/// </summary>
public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<ApiResponse<bool>>;
