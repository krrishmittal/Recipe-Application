using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the remove favorite command request.
/// </summary>
public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
{
    /// <summary>
    /// Initializes a new instance of the RemoveFavoriteCommandValidator class.
    /// </summary>
    public RemoveFavoriteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RecipeId).NotEmpty();
    }
}
