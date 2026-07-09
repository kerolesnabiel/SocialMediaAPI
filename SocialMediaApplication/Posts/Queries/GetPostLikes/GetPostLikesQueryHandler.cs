using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users.Dtos;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Posts.Queries.GetPostLikes;

public class GetPostLikesQueryHandler(ILogger<GetPostLikesQueryHandler> logger,
        IPostsRepository postsRepository)
        : IRequestHandler<GetPostLikesQuery, IEnumerable<UserMiniDto>>
{
    public async Task<IEnumerable<UserMiniDto>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting post likes list for: {PostId}", request.Id);

        var post = await postsRepository.GetByIdWithLikesAsync(request.Id)
            ?? throw new NotFoundException(nameof(Post), request.Id.ToString());

        return post.Likes.Adapt<IEnumerable<UserMiniDto>>();
    }
}
