using FluentValidation;
using Recipe.Domain.Models;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin user role updates.
/// </summary>
public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateUserRoleCommand.
    /// </summary>
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role.Equals(UserRoles.User, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Admin role cannot be assigned through the API.");
    }
}
