using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Users.Dtos;
using SocialMediaApplication.Users.Queries.GetUserById;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Users.Queries.GetUserById;

public class GetUserByIdQueryHandlerTests
{
    private Mock<ILogger<GetUserByIdQueryHandler>> _logger = new();
    private Mock<IUsersRepository> _usersRepository = new();
    private GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _handler = new(_logger.Object, _usersRepository.Object);
    }

    [Fact()]
    public async Task Handle_WithValidQuery_ReturnUserDto()
    {
        string id = "userId";
        var user = new User() { Id = id, FullName = "Name", UserName= "username",
            Posts = [ new() { Id = 1}] };
        int followersCount = 100;
        int followingCount = 50;
        var userDto = new UserDto() { Id = id, FullName = user.FullName, Username = user.UserName,
            Posts = [new() { Id = 1 }], FollowersCount = followersCount, FollowingCount = followingCount };

        _usersRepository.Setup(r => r.GetByIdWithPostsAsync(id)).ReturnsAsync(user);
        _usersRepository.Setup(r => r.GetFollowersAndFollowingCountAsync(id))
                .ReturnsAsync((followersCount, followingCount));

        var result = await _handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(followersCount, result.FollowersCount);
        Assert.Equal(followingCount, result.FollowingCount);
        Assert.NotNull(result.Posts);
        Assert.Single(result.Posts);
        Assert.Equal(1, result.Posts.First().Id);
        Assert.Equal(100, result.FollowersCount);
        Assert.Equal(50, result.FollowingCount);
        _usersRepository.Verify(r => r.GetByIdWithPostsAsync(id), Times.Once());
        _usersRepository.Verify(r => r.GetFollowersAndFollowingCountAsync(id), Times.Once());
    }
}