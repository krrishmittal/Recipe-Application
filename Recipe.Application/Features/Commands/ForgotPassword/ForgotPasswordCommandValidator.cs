using FluentValidation;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Validates the forgot password command request.
/// </summary>
public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the ForgotPasswordCommandValidator class.
    /// </summary>
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");
    }
}
