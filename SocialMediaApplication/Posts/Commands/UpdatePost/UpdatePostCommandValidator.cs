using FluentValidation;

namespace SocialMediaApplication.Posts.Commands.UpdatePost;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(p => p.Content)
            .NotEmpty()
            .When(p => p.Content != null);
        RuleForEach(p => p.Images)
            .Must(file => file.Length <= 5 * 1024 * 1024) // 5MB
            .WithMessage("Each image must be less than or equal to 5MB.")
            .Must(file => file.ContentType.StartsWith("image/"))
            .WithMessage("Only image files are allowed.");
    }
}
