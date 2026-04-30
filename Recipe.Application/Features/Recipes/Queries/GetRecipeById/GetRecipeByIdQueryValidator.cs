using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the get recipe by id query request.
/// </summary>
public class GetRecipeByIdQueryValidator : AbstractValidator<GetRecipeByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetRecipeByIdQueryValidator class.
    /// </summary>
    public GetRecipeByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
