using FluentValidation;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin user deletion requests.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    /// <summary>
    /// Initializes validation rules for DeleteUserCommand.
    /// </summary>
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
