using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Posts.Dtos;
using SocialMediaApplication.Posts.Queries.GetFeed;
using SocialMediaApplication.Users;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Posts.Queries.GetFeed;

public class GetFeedQueryHandlerTests
{
    private readonly Mock<ILogger<GetFeedQueryHandler>> _logger
    = new Mock<ILogger<GetFeedQueryHandler>>();
    private readonly Mock<IPostsRepository> _postsRepository = new();
    private readonly Mock<IUsersRepository> _usersRepository = new();
    private readonly Mock<IUserContext> _userContext = new();
    private readonly GetFeedQueryHandler _handler;

    public GetFeedQueryHandlerTests()
    {
        _handler = new GetFeedQueryHandler
            (_logger.Object, _postsRepository.Object,_usersRepository.Object, _userContext.Object);
    }

    [Fact()]
    public async Task Handle_WithValidQuery_GetPosts()
    {
        var currentUser = new CurrentUser("id", "email@e.com", "Username", []);
        var user = new User() { Id = "id" };
        List<Post> posts = [new() { Id = 1}, new() { Id = 2 }];
        List<PostDto> postsDto = [new() { Id = 1}, new() { Id = 2 }];

        _userContext.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _postsRepository.Setup(r => r.GetFeedAsync(user.Id, 10, 1, null)).ReturnsAsync((posts, 2));

        var result = await _handler.Handle(new GetFeedQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(postsDto.First().Id, result.Items.First().Id);
        Assert.Equal(postsDto.Last().Id, result.Items.Last().Id);
        Assert.Equal(2, result.TotalItemsCount);
        _postsRepository.Verify(r => r.GetFeedAsync(user.Id, 10, 1, null), Times.Once);
    }
}