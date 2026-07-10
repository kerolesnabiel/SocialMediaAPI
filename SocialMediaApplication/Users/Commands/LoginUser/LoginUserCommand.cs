using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SocialMediaApplication.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<SignInHttpResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
