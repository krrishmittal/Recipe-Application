using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates the get all recipes query request.
/// </summary>
public class GetAllRecipesQueryValidator : AbstractValidator<GetAllRecipesQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetAllRecipesQueryValidator class.
    /// </summary>
    public GetAllRecipesQueryValidator()
    {
        RuleFor(x => x.Request.Page).GreaterThan(0);
        RuleFor(x => x.Request.PageSize).GreaterThan(0);
        RuleFor(x => x.Request.SortOrder)
            .Must(v => string.IsNullOrWhiteSpace(v) || v.Equals("asc", StringComparison.OrdinalIgnoreCase) || v.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be 'asc' or 'desc'.");
        RuleFor(x => x.Request.SortBy)
            .Must(v => string.IsNullOrWhiteSpace(v) ||
                       v.Equals("title", StringComparison.OrdinalIgnoreCase) ||
                       v.Equals("preptime", StringComparison.OrdinalIgnoreCase) ||
                       v.Equals("cooktime", StringComparison.OrdinalIgnoreCase) ||
                       v.Equals("created", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortBy must be one of: title, prepTime, cookTime, created.");
    }
}
