using MediatR;
using Recipe.Application.DTOs.Response;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Requests permanent deletion of a user by an admin.
/// </summary>
public record DeleteUserCommand(Guid UserId) : IRequest<ApiResponse<bool>>;
