using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Users.Dtos;
using SocialMediaApplication.Users.Queries.SearchForUser;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Users.Queries.SearchForUser;

public class SearchForUserQueryHandlerTests
{
    private Mock<ILogger<SearchForUserQueryHandler>> _logger = new();
    private Mock<IUsersRepository> _usersRepository = new();
    private SearchForUserQueryHandler _handler;

    public SearchForUserQueryHandlerTests()
    {
        _handler = new(_logger.Object, _usersRepository.Object);
    }

    [Fact()]
    public async Task Handle_WithValidQuery_GetFollowersList()
    {
        string searchPhase = "user";
        List<User> users = [new() { Id = "user1" }, new() { Id = "user2" }];
        List<UserMiniDto> usersDto = [new() { Id = "user1" }, new() { Id = "user2" }];

        _usersRepository.Setup(r => r.FindManyContains(searchPhase)).ReturnsAsync(users);

        var result = await _handler.Handle(new() { SearchPhase = searchPhase}, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(usersDto.Count, result.Count());
        Assert.Equal(usersDto[0].Id, result.First().Id);
        Assert.Equal(usersDto[1].Id, result.Last().Id);
        _usersRepository.Verify(r => r.FindManyContains(searchPhase), Times.Once);
    }
}