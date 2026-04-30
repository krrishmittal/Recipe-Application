using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates recipe rating deletion requests.
/// </summary>
public class DeleteRecipeRatingCommandValidator : AbstractValidator<DeleteRecipeRatingCommand>
{
    public DeleteRecipeRatingCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
