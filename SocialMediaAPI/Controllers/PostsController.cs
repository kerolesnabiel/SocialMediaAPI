using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApplication.Posts.Commands.CreatePost;
using SocialMediaApplication.Posts.Commands.DeletePost;
using SocialMediaApplication.Posts.Commands.LikeOrUnlikePost;
using SocialMediaApplication.Posts.Commands.UpdatePost;
using SocialMediaApplication.Posts.Queries.GetAllPosts;
using SocialMediaApplication.Posts.Queries.GetFeed;
using SocialMediaApplication.Posts.Queries.GetPostById;
using SocialMediaApplication.Posts.Queries.GetPostLikes;

namespace SocialMediaAPI.Controllers;

[Route("api/posts")]
[ApiController]
[Authorize]
public class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetPostById), new { id }, null);
    }

    [HttpGet("feed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed([FromQuery] GetFeedQuery query)
    {
        var post = await mediator.Send(query);
        return Ok(post);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPosts([FromQuery] GetAllPostsQuery query)
    {
        var post = await mediator.Send(query);
        return Ok(post);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] int id)
    {
        var post = await mediator.Send(new GetPostByIdQuery(id));
        return Ok(post);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        await mediator.Send(new DeletePostCommand(id));
        return NoContent();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromForm] UpdatePostCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{postId}/like")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Like([FromRoute] int postId)
    {
        await mediator.Send(new LikeOrUnlikePostCommand(postId));
        return Ok();
    }

    [HttpGet("{postId}/likes")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostLikes([FromRoute] int postId)
    {
        var postLikes = await mediator.Send(new GetPostLikesQuery(postId));
        return Ok(postLikes);
    }
}
