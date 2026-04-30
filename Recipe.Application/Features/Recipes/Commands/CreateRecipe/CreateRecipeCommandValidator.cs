using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the create recipe command request.
/// </summary>
public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateRecipeCommandValidator class.
    /// </summary>
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(20).WithMessage("Description must be at least 20 characters")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Request.PrepTimeMinutes)
            .NotEmpty().WithMessage("Preparation Time is required")
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative")
            .LessThanOrEqualTo(180).WithMessage("Prep time must not be greater than 180 minutes (3 hours)");

        RuleFor(x => x.Request.CookTimeMinutes)
            .NotEmpty().WithMessage("Cooking Time is required")
            .GreaterThanOrEqualTo(0).WithMessage("Cooking time cannot be negative")
            .LessThanOrEqualTo(300).WithMessage("Cooking time must not exceed 300 minutes (5 hours)");

        RuleFor(x => x.Request.Ingredients)
            .NotEmpty().WithMessage("Ingredients are required");

        RuleFor(x => x.Request.steps)
            .NotEmpty().WithMessage("Steps are required");

        RuleFor(x => x.Request.Image)
            .NotNull().WithMessage("Image is required.")
            .Must(file =>
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                return allowed.Contains(file!.ContentType.ToLower());
            }).WithMessage("Image must be a JPEG, PNG, or WebP file.");
    }
}
