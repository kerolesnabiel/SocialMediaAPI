using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMediaApplication.Common;
using SocialMediaApplication.Users.Dtos;
using SocialMediaDomain.Interfaces;

namespace SocialMediaApplication.Users.Commands.GetAllUsers;

internal class GetAllUsersQueryHandler(ILogger<GetAllUsersQueryHandler> logger,
        IUsersRepository usersRepository) 
            : IRequestHandler<GetAllUsersQuery, PagedResult<UserMiniDto>>
{
    public async Task<PagedResult<UserMiniDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users");

        var (users, totalCount) = await usersRepository.GetAllAsync(request.PageSize, request.PageNumber);

        var miniUsers = users.Adapt<IEnumerable<UserMiniDto>>();

        return new PagedResult<UserMiniDto>(miniUsers, totalCount, request.PageSize, request.PageNumber);
    }
}
