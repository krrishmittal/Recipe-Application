using FluentValidation;

namespace Recipe.Application.Features.Auth;

/// <summary>
/// Validates the update profile command request.
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateProfileCommandValidator class.
    /// </summary>
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(4).WithMessage("Name must be at least 4 characters")
            .MaximumLength(20).WithMessage("Name must not exceed 20 characters")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.Request.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.");

        RuleFor(x => x.Request.ProfileImage)
            .Must(file =>
            {
                if (file is null)
                {
                    return true;
                }

                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                return allowed.Contains(file.ContentType.ToLower());
            })
            .WithMessage("Profile image must be a JPEG, PNG, or WebP file.");
    }
}
