using FluentValidation;
using Server.DTOs;
using Server.DTOs.Request;
using System.Data;
namespace Server.Validators;
// user validations
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required")
        .MinimumLength(4).WithMessage("Name must be at least 4 characters")
        .MaximumLength(20).WithMessage("Name must not exceed 20 characters")
        .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .MaximumLength(200).WithMessage("Email must not exceed 200 characters")
        .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required")
        .MinimumLength(8).WithMessage("Password should be atleast 8 characters")
        .Matches(@"[A-Z]").WithMessage("Password must contain atleast one uppercase character")
        .Matches(@"[a-z]").WithMessage("Password must contain atleast one lowercase character")
        .Matches(@"[0-9]").WithMessage("Password must contain atleast one number")
        .Matches(@"[\W]").WithMessage("Password must contain at least one special character");

    }
}

//login request Validator
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator() 
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required");
    }
}

// ForgotPassword Validator
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .EmailAddress().WithMessage("A valid email address is required");
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.OtpCode)
        .NotEmpty().WithMessage("Otp is required");

        RuleFor(x=>x.NewPassword)
        .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
    }
}

// Create Recipe Validator
public class CreateRecipeValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeValidator()
    {
        RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Title is required")
        .MinimumLength(3).WithMessage("Title must be at least 3 characters")
        .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
        .NotEmpty().WithMessage("Description is required")
        .MinimumLength(20).WithMessage("Description must be at least 20 characters")
        .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.PrepTimeMinutes)
        .NotEmpty().WithMessage("Preparation Time is required")
        .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative")
        .LessThanOrEqualTo(180).WithMessage("Prep time must not be greater than 180 minutes (3 hours)");

        RuleFor(x => x.CookTimeMinutes)
        .NotEmpty().WithMessage("Cooking Time is required")
        .GreaterThanOrEqualTo(0).WithMessage("Cooking time cannot be negative")
        .LessThanOrEqualTo(300).WithMessage("Cooking time must not exceed 300 minutes (5 hours)");

        RuleFor(x => x.Ingredients)
        .NotEmpty().WithMessage("Ingredients are required");

        RuleFor(x => x.steps)
        .NotEmpty().WithMessage("Steps are required");

        RuleFor(x => x.Image)
        .NotNull().WithMessage("Image is required.")
        .Must(file =>
        {
            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            return allowed.Contains(file!.ContentType.ToLower());
        }).WithMessage("Image must be a JPEG, PNG, or WebP file.");


    }
}

//update recipe validator
public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(20).WithMessage("Description must be at least 20 characters.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative.")
            .LessThanOrEqualTo(180).WithMessage("Prep time must not exceed 180 minutes (3 hours).");

        RuleFor(x => x.CookTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Cook time cannot be negative.")
            .LessThanOrEqualTo(300).WithMessage("Cook time must not exceed 300 minutes (5 hours).");

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("Ingredients are required.");

        RuleFor(x => x.steps)
            .NotEmpty().WithMessage("Steps are required.");
    }
}