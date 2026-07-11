using FluentValidation;

namespace SocialMediaApplication.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.FullName)
            .NotEmpty()
            .MaximumLength(50)
            .When(c => c.FullName != null);

        RuleFor(c => c.Bio)
            .MaximumLength(150)
            .When(c => c.Bio != null);

        RuleFor(c => c.Picture)
            .Must(file => file!.Length <= 5 * 1024 * 1024) // 5MB
            .WithMessage("Each image must be less than or equal to 5MB.")
            .Must(file => file!.ContentType.StartsWith("image/"))
            .WithMessage("Only image files are allowed.")
            .When(c => c.Picture != null);
    }
}
