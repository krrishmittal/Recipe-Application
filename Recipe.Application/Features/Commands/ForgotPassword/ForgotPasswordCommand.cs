using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to forgot password.
/// </summary>
public record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<ApiResponse<bool>>;
