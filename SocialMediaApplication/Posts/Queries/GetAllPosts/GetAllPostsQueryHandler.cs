using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Common;
using SocialMediaApplication.Posts.Dtos;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Posts.Queries.GetAllPosts;

public class GetAllPostsQueryHandler(ILogger<GetAllPostsQueryHandler> logger,
        IPostsRepository postsRepository)
            : IRequestHandler<GetAllPostsQuery, PagedResult<PostDto>>
{
    public async Task<PagedResult<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting posts in page: {PageNumber}", request.PageNumber);
        var (posts, totalCount) = await postsRepository
            .GetAllAsync(request.PageSize, request.PageNumber, request.searchPhase);

        var postDtos = posts.Adapt<IEnumerable<PostDto>>();

        var result = new PagedResult<PostDto>(postDtos, totalCount, request.PageSize, request.PageNumber);

        return result;
    }
}
