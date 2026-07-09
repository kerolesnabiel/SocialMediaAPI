using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Comments.Dtos;
using SocialMediaApplication.Users;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Comments.Queries.GetCommentById;

public class GetCommentByIdQueryHandler(ILogger<GetCommentByIdQueryHandler> logger,
        IPostsRepository postsRepository,
        ICommentsRepository commentsRepository,
        IUserContext userContext) : IRequestHandler<GetCommentByIdQuery, CommentDto>
{
    public async Task<CommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var user = userContext.GetCurrentUser();
        logger.LogInformation("Getting comment: {CommentId}, post :{PostId}, user: {UserId}",
            request.CommentId, request.PostId, user!.Id);

        var post = await postsRepository.GetByIdAsync(request.PostId)
             ?? throw new NotFoundException(nameof(Post), request.PostId.ToString());

        var comment = await commentsRepository.GetByIdAsync(request.CommentId)
             ?? throw new NotFoundException(nameof(Comment), request.CommentId.ToString());

        var result = comment.Adapt<CommentDto>();
        result.LikesCount = await commentsRepository.GetLikesCountAsync(request.CommentId);

        return result;
    }
}
