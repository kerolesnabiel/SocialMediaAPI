using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Posts.Commands.DeletePost;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Posts.Commands.DeletePost;

public class DeletePostCommandHandlerTests
{
    private readonly DeletePostCommandHandler _handler;
    private readonly Mock<ILogger<DeletePostCommandHandler>> _logger;
    private readonly Mock<IPostsRepository> _postsRepository;
    private readonly Mock<IPostAuthorizationService> _postAuthorizationService;
    private readonly Mock<IUserContext> _userContext;

    public DeletePostCommandHandlerTests()
    {
        _logger = new Mock<ILogger<DeletePostCommandHandler>>();
        _postsRepository = new Mock<IPostsRepository>();
        _postAuthorizationService = new Mock<IPostAuthorizationService>();
        _userContext = new Mock<IUserContext>();
        var currentUser = new CurrentUser("authorId", "test@test.com", "username", []);
        _userContext.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        _handler = new DeletePostCommandHandler(
            _logger.Object,
            _postsRepository.Object,
            _postAuthorizationService.Object,
            _userContext.Object
            );
    }

    [Fact()]
    public async Task Handle_WithValidRequest_DeletePost()
    {
        var id = 1;
        var command = new DeletePostCommand(id);

        var post = new Post { Id = id  };
        _postsRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _postsRepository.Setup(r => r.Delete(post));
        _postAuthorizationService.Setup(s => s.Authorize(post, ResourceOperation.Delete)).Returns(true);

        await _handler.Handle(command, CancellationToken.None);

        _postsRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        _postsRepository.Verify(r => r.Delete(post), Times.Once);
    }

    [Fact()]
    public async Task Handle_WithNonExistingPost_ThrowsNotFoundException()
    {
        var id = 0;
        var command = new DeletePostCommand(id);

        var post = new Post { Id = id };
        _postsRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(null as Post);

         var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact()]
    public async Task Handle_WithNotValidAuthor_ThrowsForbidException()
    {
        var id = 1;
        var command = new DeletePostCommand(id);

        var post = new Post { Id = id };
        _postsRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _postsRepository.Setup(r => r.Delete(post));
        _postAuthorizationService.Setup(s => s.Authorize(post, ResourceOperation.Delete)).Returns(false);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<ForbidException>(act);
    }
}