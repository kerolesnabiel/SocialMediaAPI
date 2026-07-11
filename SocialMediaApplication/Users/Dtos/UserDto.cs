using SocialMediaApplication.Posts.Dtos;

namespace SocialMediaApplication.Users.Dtos;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string? Bio { get; set; }
    public string? Picture { get; set; }
    public DateTime? CreatedAt { get; set; }

    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }

    public IEnumerable<PostDto>? Posts { get; set; }
}
