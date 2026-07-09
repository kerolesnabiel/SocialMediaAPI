using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler(ILogger<UpdatePostCommandHandler> logger,
        IPostAuthorizationService postAuthorizationService,
        IBlobStorageService blobStorageService,
        IPostsRepository postsRepository,
        IUserContext userContext) : IRequestHandler<UpdatePostCommand>
{
    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("User {UserId} updating post with Id: {PostId}", user!.Id, request.Id);

        var post = await postsRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Post), request.Id.ToString());

        var isAuthorized = postAuthorizationService.Authorize(post, ResourceOperation.Update);
        if (!isAuthorized)
            throw new ForbidException();

        request.Adapt(post);
        post.UpdatedAt = DateTime.Now;

        if (request.Images != null && request.Images.Count > 0)
        {
            post.Images = [];
            int x = DateTime.Now.GetHashCode();

            foreach (var image in request.Images)
            {
                string filename = $"post-user-{user.Id}-{x++}.jpeg";
                var stream = image.OpenReadStream();

                string imageUrl = await blobStorageService.UploadToBlobAsync
                    (stream, filename, ContainerName.PostsContainerName);
                post.Images.Add(imageUrl);
            }
        }

        await postsRepository.UpdateAsync(post);
    }
}
