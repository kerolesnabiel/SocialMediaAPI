using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialMediaDomain.Entities;

namespace SocialMediaApplication.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger,
        UserManager<User> userManager) : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering a new user");

        var user = new User()
        {
            FullName = request.FullName,
            UserName = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        return result;
    }
}
