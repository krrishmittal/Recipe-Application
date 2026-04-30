using FluentValidation;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin recipe list requests.
/// </summary>
public class GetAllRecipesForAdminQueryValidator : AbstractValidator<GetAllRecipesForAdminQuery>
{
    /// <summary>
    /// Initializes validation rules for GetAllRecipesForAdminQuery.
    /// </summary>
    public GetAllRecipesForAdminQueryValidator()
    {
        RuleFor(x => x.Request.Page).GreaterThan(0);
        RuleFor(x => x.Request.PageSize).GreaterThan(0);
    }
}
