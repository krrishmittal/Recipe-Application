using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the get favorite recipes query request.
/// </summary>
public class GetFavoriteRecipesQueryValidator : AbstractValidator<GetFavoriteRecipesQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetFavoriteRecipesQueryValidator class.
    /// </summary>
    public GetFavoriteRecipesQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
