using FluentValidation;

namespace SocialMediaApplication.Posts.Commands.CreatePost
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(p => p.Content)
                .NotEmpty();
            RuleForEach(p => p.Images)
                .Must(file => file.Length <= 5 * 1024 * 1024) // 5MB
                .WithMessage("Each image must be less than or equal to 5MB.")
                .Must(file => file.ContentType.StartsWith("image/"))
                .WithMessage("Only image files are allowed.");
        }
    }
}
