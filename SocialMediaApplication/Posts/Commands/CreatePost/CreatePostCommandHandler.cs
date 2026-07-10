using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(ILogger<CreatePostCommandHandler> logger, 
        IPostAuthorizationService postAuthorizationService,
        IBlobStorageService blobStorageService,
        IPostsRepository postsRepository, 
        IUserContext userContext) : IRequestHandler<CreatePostCommand, int>
{
    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("User {UserId} creating new post {@Post}", user!.Id, request);

        var post = request.Adapt<Post>();
        post.AuthorId = user.Id;
        post.CreatedAt = DateTime.UtcNow;

        bool isAuthorized = postAuthorizationService.Authorize(post, ResourceOperation.Create);
        if (!isAuthorized)
            throw new ForbidException();

        if(request.Images != null && request.Images.Count > 0)
        {
            post.Images = [];
            foreach (var image in request.Images)
            {
                string filename = $"post-user-{user.Id}-img-{Guid.NewGuid()}.{image.ContentType.Split('/')[1]}";
                var stream = image.OpenReadStream();

                string imageUrl = await blobStorageService.UploadToBlobAsync
                    (stream, filename, ContainerName.PostsContainerName);
                post.Images.Add(imageUrl);
            }
        }

        var postId = await postsRepository.Create(post);
        return postId;
    }
}
