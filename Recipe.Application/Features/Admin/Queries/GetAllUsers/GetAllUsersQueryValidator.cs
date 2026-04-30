using FluentValidation;

namespace Recipe.Application.Features.Admin;

/// <summary>
/// Validates admin user list requests.
/// </summary>
public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    /// <summary>
    /// Initializes validation rules for GetAllUsersQuery.
    /// </summary>
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.Request.Page).GreaterThan(0);
        RuleFor(x => x.Request.PageSize).GreaterThan(0);
    }
}
