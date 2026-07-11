using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Posts.Commands.CreatePost;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Posts.Commands.CreatePost;

public class CreatePostCommandHandlerTests
{
    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsCreatedPostId()
    {
        var logger = new Mock<ILogger<CreatePostCommandHandler>>();
        var command = new CreatePostCommand();

        var postRepository = new Mock<IPostsRepository>();
        postRepository.Setup(repo => repo.Create(It.IsAny<Post>())).ReturnsAsync(1);

        var userContext = new Mock<IUserContext>();
        var currentUser = new CurrentUser("authorId", "test@test.com", "username", []);
        userContext.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var postAuthorizationService = new Mock<IPostAuthorizationService>();
        postAuthorizationService
            .Setup(s => s.Authorize(It.IsAny<Post>(), ResourceOperation.Create))
            .Returns(true);
        Mock<IBlobStorageService> blobStorageService = new();

        var handler = new CreatePostCommandHandler(
            logger.Object,
            postAuthorizationService.Object,
            blobStorageService.Object,
            postRepository.Object,
            userContext.Object
            );

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result);
        postRepository.Verify(r => r.Create(It.Is<Post>(p => p.AuthorId == "authorId")), Times.Once);
    }
}