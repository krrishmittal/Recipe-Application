using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the delete recipe command request.
/// </summary>
public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    /// <summary>
    /// Initializes a new instance of the DeleteRecipeCommandValidator class.
    /// </summary>
    public DeleteRecipeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
