using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates recipe comment creation requests.
/// </summary>
public class AddRecipeCommentCommandValidator : AbstractValidator<AddRecipeCommentCommand>
{
    public AddRecipeCommentCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
    }
}
