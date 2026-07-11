using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaApplication.Posts.Commands.UpdatePost;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaApplicationTests.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandlerTests
{
    private readonly Mock<ILogger<UpdatePostCommandHandler>> _logger = new();
    private readonly Mock<IPostAuthorizationService> _postAuthorizationService = new();
    private readonly Mock<IBlobStorageService> _blobStorageService = new();
    private readonly Mock<IPostsRepository> _postRepository = new();
    private readonly Mock<IUserContext> _userContext = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly UpdatePostCommandHandler _handler;

    public UpdatePostCommandHandlerTests()
    {
        _handler = new UpdatePostCommandHandler(
            _logger.Object,
            _postAuthorizationService.Object,
            _blobStorageService.Object,
            _postRepository.Object,
            _userContext.Object);

        var currentUser = new CurrentUser("AuthorId", "test@test.com", "username", []);
        _userContext.Setup(u => u.GetCurrentUser()).Returns(currentUser);
    }

    [Fact()]
    public async Task Handle_WithValidCommand_UpdatePost()
    {
        int id = 1;
        var command = new UpdatePostCommand() { Id = id};
        var post = new Post() { Id = id, AuthorId = "AuthorId" };

        _mapper.Setup(m => m.Map<Post>(command)).Returns(post);
        _postRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(post);
        _postRepository.Setup(repo => repo.UpdateAsync(post));
        _postAuthorizationService.Setup(s => s.Authorize(post, ResourceOperation.Update)).Returns(true);


        await _handler.Handle(command, CancellationToken.None);

        _postRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        _postRepository.Verify(r => r.UpdateAsync(post), Times.Once);
    }

    [Fact()]
    public async Task Handle_WithNotValidAuthor_UpdatePost()
    {
        int id = 1;
        var command = new UpdatePostCommand() { Id = id };
        var post = new Post() { Id = id, AuthorId = "DifferentAuthorId" };

        _postRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(post);
        _postAuthorizationService.Setup(s => s.Authorize(post, ResourceOperation.Update)).Returns(false);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<ForbidException>(act);
    }


    [Fact()]
    public async Task Handle_WithNonExistingPost_UpdatePost()
    {
        int id = 0;
        var command = new UpdatePostCommand() { Id = id };

        _postRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(null as Post);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}