using SocialMediaDomain.Entities;

namespace SocialMediaDomain.Interfaces;

public interface IPostsRepository
{
    Task<int> Create(Post entity);
    Task<Post?> GetByIdAsync(int id);
    Task<Post?> GetByIdWithLikesAsync(int id);
    Task Delete(Post entity);
    Task UpdateAsync(Post post);
    Task<(IEnumerable<Post>, int)> GetAllAsync(int pageSize, int pageNumber, string? searchPhase);
    Task<(IEnumerable<Post>, int)> GetFeedAsync(string userId, int pageSize, int pageNumber, string? searchPhase);
    Task AddLikeAsync(Post post, User user);
    Task RemoveLikeAsync(Post post, User user);
    Task<int> GetLikesCountAsync(int postId);
}
