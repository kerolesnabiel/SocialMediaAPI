using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Common;
using SocialMediaApplication.Posts.Dtos;
using SocialMediaApplication.Users;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Posts.Queries.GetFeed;

public class GetFeedQueryHandler(ILogger<GetFeedQueryHandler> logger,
        IPostsRepository postsRepository,
        IUsersRepository usersRepository,
        IUserContext userContext) : IRequestHandler<GetFeedQuery, PagedResult<PostDto>>
{
    public async Task<PagedResult<PostDto>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        logger.LogInformation("Getting feed for user: {UserId}, Page: {PageNumber}",
            currentUser!.Id, request.PageNumber);

        var user = await usersRepository.GetByIdWithFollowingAsync(currentUser.Id)
            ?? throw new NotFoundException(nameof(User), currentUser.Id);

        var (posts, totalCount) = await postsRepository
            .GetFeedAsync(user, request.PageSize, request.PageNumber, request.searchPhase);

        var postDtos = posts.Adapt<IEnumerable<PostDto>>(); 

        var result = new PagedResult<PostDto>(postDtos, totalCount, request.PageSize, request.PageNumber);

        return result;
    }
}
