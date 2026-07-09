using Mapster;
using SocialMediaApplication.Users.Commands.UpdateUser;
using SocialMediaDomain.Entities;

namespace SocialMediaApplication.Users.Dtos;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateUserCommand, User>().IgnoreNullValues(true);
    }
}
