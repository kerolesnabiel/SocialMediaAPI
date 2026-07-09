using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Users.Dtos;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Exceptions;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger,
    IUsersRepository usersRepository) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user with Id: {UserId}", request.Id);

        var user = await usersRepository.GetByIdWithPostsAsync(request.Id)
            ?? throw new NotFoundException(nameof(User), request.Id);

        var (FollowersCount, FollowingCount) = await usersRepository.GetFollowersAndFollowingCountAsync(request.Id);

        var result = user.Adapt<UserDto>();
        result.FollowersCount = FollowersCount;
        result.FollowingCount = FollowingCount;

        return result;
    }
}
