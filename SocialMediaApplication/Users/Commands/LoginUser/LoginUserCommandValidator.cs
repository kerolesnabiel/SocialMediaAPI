using FluentValidation;

namespace SocialMediaApplication.Users.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(c => c.Username)
            .NotEmpty();
        RuleFor(c => c.Password)
            .NotEmpty();
    }
}
