using FluentValidation;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin featured status updates.
/// </summary>
public class UpdateRecipeFeaturedStatusCommandValidator : AbstractValidator<UpdateRecipeFeaturedStatusCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateRecipeFeaturedStatusCommand.
    /// </summary>
    public UpdateRecipeFeaturedStatusCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
    }
}
