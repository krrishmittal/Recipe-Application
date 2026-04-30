using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates recipe rating requests.
/// </summary>
public class RateRecipeCommandValidator : AbstractValidator<RateRecipeCommand>
{
    public RateRecipeCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Value).InclusiveBetween(1, 5);
    }
}
