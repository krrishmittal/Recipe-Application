using FluentValidation;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Validates the delete account command request.
/// </summary>
public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    /// <summary>
    /// Initializes a new instance of the DeleteAccountCommandValidator class.
    /// </summary>
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
