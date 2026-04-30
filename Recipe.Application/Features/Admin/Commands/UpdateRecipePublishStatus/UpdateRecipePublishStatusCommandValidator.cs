using FluentValidation;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin publish status updates.
/// </summary>
public class UpdateRecipePublishStatusCommandValidator : AbstractValidator<UpdateRecipePublishStatusCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateRecipePublishStatusCommand.
    /// </summary>
    public UpdateRecipePublishStatusCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
    }
}
