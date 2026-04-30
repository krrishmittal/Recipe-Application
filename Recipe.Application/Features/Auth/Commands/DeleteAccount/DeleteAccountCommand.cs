using MediatR;
using Recipe.Application.DTOs.Request;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Represents the command used to delete account.
/// </summary>
public record DeleteAccountCommand(Guid UserId, DeleteAccountRequest Request) : IRequest<ApiResponse<bool>>;
