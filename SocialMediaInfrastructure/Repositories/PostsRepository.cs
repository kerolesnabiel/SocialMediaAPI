using Microsoft.EntityFrameworkCore;
using SocialMediaDomain.Entities;
using SocialMediaDomain.Interfaces;
using SocialMediaInfrastructure.Persistence;

namespace SocialMediaInfrastructure.Repositories;

internal class PostsRepository(SocialMediaDbContext dbContext) : IPostsRepository
{
    public async Task<int> Create(Post entity)
    {
        dbContext.Posts.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(Post entity)
    {
        var comments = await dbContext.Comments.Include(c => c.Likes)
                .Where(c => c.PostId == entity.Id).ToListAsync();

        foreach (var comment in comments)
            comment.Likes.Clear();

        dbContext.RemoveRange(comments);

        entity.Likes.Clear();
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Post>, int)> GetAllAsync(int pageSize, int pageNumber, string? searchPhase)
    {
        var baseQuery = dbContext.Posts
            .Where(p => searchPhase == null || p.Content.Contains(searchPhase, StringComparison.CurrentCultureIgnoreCase));

        var totalCount = await baseQuery.CountAsync();

        var posts = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (posts, totalCount);
    }

    public async Task<(IEnumerable<Post>, int)> GetFeedAsync(string userId, int pageSize, int pageNumber, string? searchPhase)
    {
        var baseQuery = dbContext.Posts
            .Where(p => p.Author.Followers.Any(f => f.Id == userId))
            .Where(p => searchPhase == null || p.Content.Contains(searchPhase, StringComparison.CurrentCultureIgnoreCase))
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await baseQuery.CountAsync();

        var posts = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (posts, totalCount);
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        dbContext.Update(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Post?> GetByIdWithLikesAsync(int id)
    {
        var post = await dbContext.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);
        return post;
    }

    public async Task AddLikeAsync(Post post, User user)
    {
        post.Likes.Add(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveLikeAsync(Post post, User user)
    {
        post.Likes.Remove(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> GetLikesCountAsync(int postId)
    {
        var count = await dbContext.Posts.
            Where(p => p.Id == postId)
            .Select(p => p.Likes.Count)
            .FirstOrDefaultAsync();

        return count;
    }
}
