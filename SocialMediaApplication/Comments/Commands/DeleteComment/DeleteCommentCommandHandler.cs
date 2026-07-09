using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler(ILogger<DeleteCommentCommandHandler> logger,
        ICommentAuthorizationService commentAuthorizationService,
        ICommentsRepository commentsRepository,
        IPostsRepository postsRepository,
        IUserContext userContext) : IRequestHandler<DeleteCommentCommand>
{
    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("Deleting comment: {CommentId}, On post: {PostId}, By user: {UserId}",
            request.CommentId, request.PostId, user!.Id);

        var post = await postsRepository.GetByIdAsync(request.PostId)
            ?? throw new NotFoundException(nameof(Post), request.PostId.ToString());

        var comment = await commentsRepository.GetByIdWithLikesAsync(request.CommentId)
            ?? throw new NotFoundException(nameof(Comment), request.CommentId.ToString());

        var isAuthorized = commentAuthorizationService.Authorize(comment, post, ResourceOperation.Delete);
        if (!isAuthorized) throw new ForbidException();
        
        await commentsRepository.DeleteAsync(comment);
    }
}
