using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Posts.Dtos;
using SocialMediaApplication.Posts.Queries.GetAllPosts;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Posts.Queries.GetAllPosts;

public class GetAllPostsQueryHandlerTests
{
    private readonly Mock<ILogger<GetAllPostsQueryHandler>> _logger = new();
    private readonly Mock<IPostsRepository> _postsRepository = new();
    private readonly GetAllPostsQueryHandler _handler;

    public GetAllPostsQueryHandlerTests()
    {
        _handler = new GetAllPostsQueryHandler
            (_logger.Object, _postsRepository.Object);
    }

    [Fact()]
    public async Task Handle_WithValidQuery_ReturnListOfPosts() 
    { 
        IEnumerable<Post> posts = [new() { Id = 1}, new() { Id = 2}];
        IEnumerable<PostDto> postsDto = [new() { Id = 1}, new() { Id = 2}];

        _postsRepository.Setup(r => r.GetAllAsync(10, 1, null)).ReturnsAsync((posts, 2));

        var result = await _handler.Handle(new GetAllPostsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(postsDto.First().Id, result.Items.First().Id);
        Assert.Equal(postsDto.Last().Id, result.Items.Last().Id);
        Assert.Equal(2, result.TotalItemsCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.ItemsFrom);
        Assert.Equal(10, result.ItemsTo);
        _postsRepository.Verify(r => r.GetAllAsync(10, 1, null), Times.Once);
    }
}