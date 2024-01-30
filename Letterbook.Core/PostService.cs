using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Medo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Letterbook.Core;

/// <summary>
/// The PostService is how you Post™
/// </summary>
public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;
    private readonly CoreOptions _options;
    private readonly IAccountProfileAdapter _profiles;
    private readonly IPostAdapter _posts;

    public PostService(ILogger<PostService> logger, IOptions<CoreOptions> options, IAccountProfileAdapter profiles, IPostAdapter posts)
    {
        _logger = logger;
        _profiles = profiles;
        _posts = posts;
        _options = options.Value;
    }

    public async Task<Post> LookupPost(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> DraftNote(Uuid7 authorId, string contentSource, Uuid7? inReplyToId = default)
    {
        var author = await _profiles.LookupProfile(authorId)
                     ?? throw CoreException.MissingData($"Couldn't find profile {authorId}", typeof(Profile), authorId);
        var post = new Post(_options);
        var note = new Note
        {
            Content = contentSource,
            Post = post,
            FediId = default!
        };
        post.Creators.Add(author);
        note.SetLocalFediId(_options);
        note.GeneratePreview();
        post.AddContent(note);

        return await Draft(post, inReplyToId);
    }

    public async Task<Post> Draft(Post post, Uuid7? inReplyToId = default)
    {
        if (inReplyToId is { } parentId)
        {
            var parent = await _posts.LookupPost(parentId); 
            if (parent is null) 
                throw CoreException.MissingData($"Couldn't find post {parentId} to reply to", typeof(Post), parentId);
            post.InReplyTo = parent;
            post.Thread = parent.Thread;
            post.Thread.Posts.Add(post);
        }

        _posts.Add(post);
        await _posts.Commit();

        return post;
    }

    public async Task<Post> Update(Post post)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> Publish(Uuid7 id, bool localOnly = false)
    {
        throw new NotImplementedException();
    }

    public async Task Share(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task Like(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> AddContent(Uuid7 postId, IContent content)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> UpdateContent(Uuid7 postId, IContent content)
    {
        throw new NotImplementedException();
    }
}