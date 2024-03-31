using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IPostAdapter
{
    public Task<Post?> LookupPost(Uuid7 postId);
    public Task<Post?> LookupPost(Uri fediId);
    public Task<ThreadContext?> LookupThread(Uri threadId);
    public Task<ThreadContext?> LookupThread(Uuid7 threadId);
    public Task<Post?> LookupPostWithThread(Uuid7 postId);
    public Task<Post?> LookupPostWithThread(Uri postId);
    public Task<Profile?> LookupProfile(Uuid7 profileId);
    public Task<Profile?> LookupProfile(Uri profileId);
    public void Add(Post post);
    public void Add(Profile profile);
    public void AddRange(IEnumerable<Post> posts);
    public void Update(Post post);
    public void Update(Profile profile);
    public void UpdateRange(IEnumerable<Post> post);
    public void Remove(Post post);
    public void Remove(Content content);
    public void RemoveRange(IEnumerable<Post> posts);
    public void RemoveRange(IEnumerable<Content> contents);

    public Task Cancel();
    public Task Commit();
}