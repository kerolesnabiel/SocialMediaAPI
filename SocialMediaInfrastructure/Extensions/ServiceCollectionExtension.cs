using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using SocialMediaInfrastructure.Persistence;
using SocialMediaInfrastructure.Repositories;
using SocialMediaInfrastructure.Seeders;
using SocialMediaInfrastructure.Services;
using SocialMediaInfrastructure.Storage;

namespace SocialMediaInfrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SocialMediaDb");
        services.AddDbContext<SocialMediaDbContext>(
            options => options.UseSqlServer(connectionString));

        services.AddIdentityApiEndpoints<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SocialMediaDbContext>();

        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddScoped<IPostsRepository, PostsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddScoped<IPostAuthorizationService, PostAuthorizationService>();
        services.AddScoped<ICommentAuthorizationService, CommentAuthorizationService>();

        services.AddScoped<ISeeder, Seeder>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString("BlobStorage")!));
    }
}
