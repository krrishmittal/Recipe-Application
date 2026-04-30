using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the add favorite command request.
/// </summary>
public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
{
    /// <summary>
    /// Initializes a new instance of the AddFavoriteCommandValidator class.
    /// </summary>
    public AddFavoriteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RecipeId).NotEmpty();
    }
}
