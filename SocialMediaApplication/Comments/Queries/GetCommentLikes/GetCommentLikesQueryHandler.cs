using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users.Dtos;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Comments.Queries.GetCommentLikes;

public class GetCommentLikesQueryHandler(ILogger<GetCommentLikesQueryHandler> logger,
        IPostsRepository postsRepository,
        ICommentsRepository commentsRepository) : IRequestHandler<GetCommentLikesQuery, IEnumerable<UserMiniDto>>
{
    public async Task<IEnumerable<UserMiniDto>> Handle(GetCommentLikesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting comment user likes: {CommentId}, post :{PostId}", request.CommentId, request.PostId);

        var post = await postsRepository.GetByIdAsync(request.PostId)
             ?? throw new NotFoundException(nameof(Post), request.PostId.ToString());

        var comment = await commentsRepository.GetByIdWithLikesAsync(request.CommentId)
             ?? throw new NotFoundException(nameof(Comment), request.CommentId.ToString());
        
        return comment.Likes.Adapt<IEnumerable<UserMiniDto>>();
    }
}
