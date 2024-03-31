using Letterbook.Core.Adapters;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.Db;

public class PostAdapter : IPostAdapter, IAsyncDisposable
{
    private readonly RelationalContext _context;
    private readonly ILogger<PostAdapter> _logger;

    public PostAdapter(ILogger<PostAdapter> logger, RelationalContext context)
    {
        _logger = logger;
        _context = context;
    }

    private static IQueryable<Models.Post> WithDefaults(IQueryable<Models.Post> query) =>
        query.Include(p => p.Creators).Include(p => p.Contents);

    private static IQueryable<Models.Post> WithThread(IQueryable<Models.Post> query) => WithDefaults(query)
        .Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Creators)
        .Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Contents)
        .AsSplitQuery();

    public async Task<Models.Post?> LookupPost(Uuid7 postId)
    {
        return await WithDefaults(_context.Posts).FirstOrDefaultAsync(post => post.Id == postId.ToGuid());
    }

    public async Task<Models.Post?> LookupPost(Uri fediId)
    {
        return await WithDefaults(_context.Posts).FirstOrDefaultAsync(post => post.FediId == fediId);
    }

    public async Task<Models.ThreadContext?> LookupThread(Uri threadId)
    {
        return await _context.Threads
            .Include(thread => thread.Posts)
            .FirstOrDefaultAsync(thread => thread.FediId == threadId);
    }

    public async Task<Models.ThreadContext?> LookupThread(Uuid7 threadId)
    {
        return await _context.Threads
            .Include(thread => thread.Posts)
            .FirstOrDefaultAsync(thread => thread.Id == threadId.ToGuid());
    }

    public async Task<Models.Post?> LookupPostWithThread(Uuid7 postId)
    {
        return await WithThread(_context.Posts)
            .FirstOrDefaultAsync(post => post.Id == postId.ToGuid());
    }

    public async Task<Models.Post?> LookupPostWithThread(Uri postId)
    {
        return await WithThread(_context.Posts)
            .FirstOrDefaultAsync(post => post.FediId == postId);
    }

    public async Task<Models.Profile?> LookupProfile(Uuid7 profileId)
    {
        return await _context.Profiles
            .FirstOrDefaultAsync(profile => profile.Id == profileId.ToGuid());
    }

    public async Task<Models.Profile?> LookupProfile(Uri profileId)
    {
        return await _context.Profiles
            .FirstOrDefaultAsync(profile => profile.FediId == profileId);
    }

    public void Add(Models.Post post) => _context.Posts.Add(post);

    public void Add(Models.Profile profile) => _context.Profiles.Add(profile);

    public void AddRange(IEnumerable<Models.Post> posts) => _context.Posts.AddRange(posts);

    public void Update(Models.Post post) => _context.Posts.Update(post);

    public void Update(Models.Profile profile) => _context.Profiles.Update(profile);

    public void UpdateRange(IEnumerable<Models.Post> post) => _context.Posts.AddRange(post);

    public void Remove(Models.Post post) => _context.Posts.Remove(post);

    public void Remove(Models.Content content) => _context.Remove(content);

    public void RemoveRange(IEnumerable<Models.Post> posts) => _context.Posts.RemoveRange(posts);

    public void RemoveRange(IEnumerable<Models.Content> contents) => _context.RemoveRange(contents);

    public Task Cancel()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return _context.Database.RollbackTransactionAsync();
        }

        return Task.CompletedTask;
    }

    public Task Commit()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return _context.Database.CommitTransactionAsync();
        }

        return _context.SaveChangesAsync();
    }

    private void Start()
    {
        if (_context.Database.CurrentTransaction is null)
        {
            _context.Database.BeginTransaction();
        }
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}