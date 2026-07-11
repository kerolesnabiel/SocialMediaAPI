using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using System.Security.Claims;

namespace SocialMediaApplication.Users.Commands.LoginUser;

public class LoginUserCommandHandler(
        ILogger<LoginUserCommandHandler> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager) : IRequestHandler<LoginUserCommand, ClaimsPrincipal>
{
    public async Task<ClaimsPrincipal> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login user");

        var user = await userManager.FindByNameAsync(request.Username) 
            ?? throw new BadRequestException("Invalid username or password");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
            throw new BadRequestException("Invalid username or password");

        return await signInManager.CreateUserPrincipalAsync(user);
    }
}
