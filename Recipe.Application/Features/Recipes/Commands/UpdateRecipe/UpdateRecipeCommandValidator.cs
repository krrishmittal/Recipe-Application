using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the update recipe command request.
/// </summary>
public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateRecipeCommandValidator class.
    /// </summary>
    public UpdateRecipeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(20).WithMessage("Description must be at least 20 characters.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Request.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative.")
            .LessThanOrEqualTo(180).WithMessage("Prep time must not exceed 180 minutes (3 hours).");

        RuleFor(x => x.Request.CookTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Cook time cannot be negative.")
            .LessThanOrEqualTo(300).WithMessage("Cook time must not exceed 300 minutes (5 hours).");

        RuleFor(x => x.Request.Ingredients)
            .NotEmpty().WithMessage("Ingredients are required.");

        RuleFor(x => x.Request.steps)
            .NotEmpty().WithMessage("Steps are required.");
    }
}
