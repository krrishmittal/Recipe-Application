using FluentValidation;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Validates the register command request.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>
    /// Initializes a new instance of the RegisterCommandValidator class.
    /// </summary>
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(4).WithMessage("Name must be at least 4 characters")
            .MaximumLength(20).WithMessage("Name must not exceed 20 characters")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters")
            .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password should be atleast 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain atleast one uppercase character")
            .Matches(@"[a-z]").WithMessage("Password must contain atleast one lowercase character")
            .Matches(@"[0-9]").WithMessage("Password must contain atleast one number")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character");
    }
}
