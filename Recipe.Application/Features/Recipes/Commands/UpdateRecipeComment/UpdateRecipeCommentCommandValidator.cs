using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates recipe comment update requests.
/// </summary>
public class UpdateRecipeCommentCommandValidator : AbstractValidator<UpdateRecipeCommentCommand>
{
    public UpdateRecipeCommentCommandValidator()
    {
        RuleFor(x => x.CommentId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
    }
}
