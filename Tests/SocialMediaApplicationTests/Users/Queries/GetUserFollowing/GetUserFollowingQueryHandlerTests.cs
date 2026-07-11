using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Users.Dtos;
using SocialMediaApplication.Users.Queries.GetUserFollowing;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Users.Queries.GetUserFollowing;

public class GetUserFollowingQueryHandlerTests
{
    private Mock<ILogger<GetUserFollowingQueryHandler>> _logger = new();
    private Mock<IUsersRepository> _usersRepository = new();
    private GetUserFollowingQueryHandler _handler;

    public GetUserFollowingQueryHandlerTests()
    {
        _handler = new(_logger.Object, _usersRepository.Object);
    }

    [Fact()]
    public async Task Handle_WithValidQuery_GetFollowingList()
    {
        string id = "userId";
        List<User> following = [new() { Id = "user1" }, new() { Id = "user2" }];
        List<UserMiniDto> followingDto = [new() { Id = "user1" }, new() { Id = "user2" }];
        User user = new() { Id = id, Following = following };

        _usersRepository.Setup(r => r.GetByIdWithFollowingAsync(id)).ReturnsAsync(user);

        var result = await _handler.Handle(new(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(followingDto.Count, result.Count());
        Assert.Equal(followingDto[0].Id, result.First().Id);
        Assert.Equal(followingDto[1].Id, result.Last().Id);
        _usersRepository.Verify(r => r.GetByIdWithFollowingAsync(id), Times.Once);
    }
}