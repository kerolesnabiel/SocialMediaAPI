using MediatR;
using Microsoft.AspNetCore.Http;


namespace SocialMediaApplication.Posts.Commands.UpdatePost;

public class UpdatePostCommand : IRequest
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public IFormFileCollection? Images { get; set; }
}
