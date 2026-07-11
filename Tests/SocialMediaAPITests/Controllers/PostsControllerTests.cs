using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SocialMediaApplication.Users;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using System.Net;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaAPITests.Controllers;

public class PostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<IPostsRepository> _postRepository = new();
    private Mock<IUsersRepository> _usersRepository = new();
    private Mock<IUserContext> _userContext = new();

    public PostsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IPostsRepository),
                    _ => _postRepository.Object));
                services.Replace(ServiceDescriptor.Scoped(typeof(IUsersRepository),
                    _ => _usersRepository.Object));
                services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext),
                    _ => _userContext.Object));
            });
        });

        var fakeUser = new CurrentUser("AuthorId", "username", "test@test.com", []);
        _userContext.Setup(u => u.GetCurrentUser()).Returns(fakeUser);
    }

    [Fact()]
    public async Task CreatePost_ForValidRequest_Return201Created()
    {
        _postRepository.Setup(r => r.Create(It.IsAny<Post>())).ReturnsAsync(1);

        var client = _factory.CreateClient();

        var formData = new MultipartFormDataContent
        {
            { new StringContent("Post Content"), "Content" }
        };

        var response = await client.PostAsync($"api/posts", formData);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact()]
    public async Task GetAllPosts_WithValidRequest_Returns200Ok()
    {
        IEnumerable<Post> posts = [new() { Id = 1 }, new() { Id = 2 }];
        _postRepository.Setup(r => r.GetAllAsync(15, 1, null)).ReturnsAsync((posts, 2));

        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/posts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact()]
    public async Task GetFeed_WithValidRequest_Returns200Ok()
    {
        User user = new() { Id = "AuthorId" };
        IEnumerable<Post> posts = [new() { Id = 1 }, new() { Id = 2 }];

        _usersRepository.Setup(r => r.GetByIdWithFollowingAsync(user.Id)).ReturnsAsync(user);
        _postRepository.Setup(r => r.GetFeedAsync(user.Id,10, 1, null)).ReturnsAsync((posts, 2));

        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/posts/feed");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact()]
    public async Task GetPostById_ForValidRequest_Return200Ok()
    {
        var id = 1;
        _postRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Post());

        var client = _factory.CreateClient();

        var response = await client.GetAsync($"api/posts/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact()]
    public async Task DeletePost_ForValidRequest_Return204NoContent()
    {
        var id = 1;
        var post = new Post() { Id = id, AuthorId = "AuthorId" };
        _postRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _postRepository.Setup(r => r.Delete(post));

        var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"api/posts/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact()]
    public async Task UpdatePost_ForValidRequest_Return204NoContent()
    {
        var id = 1;
        var post = new Post() { Id = id , AuthorId = "AuthorId" };

        _postRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _postRepository.Setup(r => r.UpdateAsync(post));

        var client = _factory.CreateClient();

        var formData = new MultipartFormDataContent
        {
            { new StringContent("Post Content"), "Content" }
        };

        var response = await client.PatchAsync($"api/posts/{id}", formData);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}