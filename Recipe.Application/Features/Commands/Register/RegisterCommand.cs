using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to register.
/// </summary>
public record RegisterCommand(RegisterRequest Request) : IRequest<ApiResponse<AuthResponse>>;
