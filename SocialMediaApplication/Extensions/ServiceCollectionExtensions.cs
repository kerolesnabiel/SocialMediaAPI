using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SocialMediaApplication.Behaviors;
using SocialMediaApplication.Users;
using SocialMediaApplication.Users.Dtos;

namespace SocialMediaApplication.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        TypeAdapterConfig.GlobalSettings.Scan(typeof(UserMappingConfig).Assembly);

        services.AddMediatR(cfg => cfg
                    .RegisterServicesFromAssembly(applicationAssembly)
                    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

        services.AddValidatorsFromAssembly(applicationAssembly);
        
        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();

    }
}
