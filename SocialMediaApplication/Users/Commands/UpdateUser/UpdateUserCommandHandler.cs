using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger,
        IBlobStorageService blobStorageService,
        IUsersRepository usersRepository,
        UserManager<User> userManager,
        IUserContext userContext) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        logger.LogInformation("Updating User with Id: {UserId}, {Updated}", currentUser!.Id, request);

        var user = await userManager.FindByIdAsync(currentUser!.Id)
            ?? throw new NotFoundException(nameof(User), currentUser.Id);

         TypeAdapterConfig<UpdateUserCommand, User>
        .NewConfig()
        .IgnoreNullValues(true); 

        request.Adapt(user);

        if (request.Picture != null) 
        {
            using var stream = request.Picture.OpenReadStream();
            string filename = $"user-{user.Id}-{DateTime.Now.GetHashCode()}.jpeg";

            var pictureUrl = await blobStorageService.UploadToBlobAsync
                (stream, filename, ContainerName.UsersContainerName);

            user.Picture = pictureUrl;
        }

        await usersRepository.UpdateAsync(user);
    }
}
