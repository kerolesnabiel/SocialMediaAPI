using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger,
        IUsersRepository usersRepository,
        UserManager<User> userManager,
        IUserContext userContext) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        logger.LogInformation("Deleting my account - user: {UserId}", currentUser!.Id);

        var user = await userManager.FindByIdAsync(currentUser.Id)
            ?? throw new NotFoundException(nameof(User), currentUser.Id);

        bool passwordIsCorrect = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordIsCorrect)
            throw new BadRequestException("Incorrect password");

        await usersRepository.DeleteAsync(user);
    }
}
