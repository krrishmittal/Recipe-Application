using FluentValidation;

namespace Recipe.Application.Features.Recipes;

/// <summary>
/// Validates recipe comment deletion requests.
/// </summary>
public class DeleteRecipeCommentCommandValidator : AbstractValidator<DeleteRecipeCommentCommand>
{
    public DeleteRecipeCommentCommandValidator()
    {
        RuleFor(x => x.CommentId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
