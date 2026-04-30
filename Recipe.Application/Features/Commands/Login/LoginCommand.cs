using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to login.
/// </summary>
public record LoginCommand(LoginRequest Request) : IRequest<ApiResponse<AuthResponse>>;
