using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users;
using SocialMediaDomain.Constants;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler(ILogger<UpdateCommentCommandHandler> logger,
        ICommentAuthorizationService commentAuthorizationService,
        ICommentsRepository commentsRepository,
        IPostsRepository postsRepository,
        IUserContext userContext) : IRequestHandler<UpdateCommentCommand>
{
    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("Updating comment: {CommentId} with {@Comment}, post: {PostId}, user: {UserId}",
                request.Id, request.Content, request.PostId, user!.Id);

        var post = await postsRepository.GetByIdAsync(request.PostId)
            ?? throw new NotFoundException(nameof(Post), request.PostId.ToString());

        var comment = await commentsRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Comment), request.Id.ToString());

        var isAuthorized = commentAuthorizationService.Authorize(comment, post, ResourceOperation.Update);
        if (!isAuthorized) throw new ForbidException();

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        await commentsRepository.UpdateAsync(comment);
    }
}
