using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users.Dtos;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Users.Queries.GetUserFollowing;

public class GetUserFollowingQueryHandler(ILogger<GetUserFollowingQueryHandler> logger,
        IUsersRepository usersRepository) : IRequestHandler<GetUserFollowingQuery, IEnumerable<UserMiniDto>>
{
    public async Task<IEnumerable<UserMiniDto>> Handle(GetUserFollowingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting following for user: {UserId}", request.Id);

        var user = await usersRepository.GetByIdWithFollowingAsync(request.Id)
            ?? throw new NotFoundException(nameof(User), request.Id);

        return user.Following.Adapt<IEnumerable<UserMiniDto>>();
    }
}
