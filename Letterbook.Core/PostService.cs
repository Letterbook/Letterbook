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

    public PostService(ILogger<PostService> logger, IOptions<CoreOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<Post> LookupPost(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> DraftNote(Profile author, string contentSource)
    {
        var post = new Post(_options);
        var note = new Note
        {
            Content = contentSource,
            Post = post,
            FediId = default!
        };
        note.SetLocalFediId(_options);
        note.GeneratePreview();
        post.AddContent(note);

        return post;
    }

    public async Task<Post> Draft(Post post, Uuid7? inReplyToId = default, Uuid7 threadId = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> Update(Post post)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Uuid7 id)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> Publish(Uuid7 id)
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