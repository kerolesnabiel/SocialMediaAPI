using MediatR;
using System.Security.Claims;

namespace SocialMediaApplication.Users.Commands.LoginUser;

public class LoginUserCommand : IRequest<ClaimsPrincipal>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
