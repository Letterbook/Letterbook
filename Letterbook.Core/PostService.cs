﻿using System.Runtime.CompilerServices;
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
    private readonly IPostAdapter _posts;
    private readonly IPostEventService _postEvents;
    private readonly IActivityPubClient _apClient;

    public PostService(ILogger<PostService> logger, IOptions<CoreOptions> options,
        IPostAdapter posts, IPostEventService postEvents, IActivityPubClient apClient)
    {
        _logger = logger;
        _posts = posts;
        _postEvents = postEvents;
        _apClient = apClient;
        _options = options.Value;
    }

    public async Task<Post?> LookupPost(Uuid7 id, bool withThread = true)
    {
        return withThread
            ? await _posts.LookupPostWithThread(id)
            : await _posts.LookupPost(id);
    }

    public async Task<Post?> LookupPost(Uri fediId, bool withThread = true)
    {
        return withThread
            ? await _posts.LookupPostWithThread(fediId)
            : await _posts.LookupPost(fediId);
    }

    public async Task<ThreadContext?> LookupThread(Uuid7 threadId)
    {
        return await _posts.LookupThread(threadId);
    }

    public async Task<ThreadContext?> LookupThread(Uri threadId)
    {
        return await _posts.LookupThread(threadId);
    }

    public async Task<Post> DraftNote(Uuid7 authorId, string contentSource, Uuid7? inReplyToId = default)
    {
        var author = await _posts.LookupProfile(authorId)
                     ?? throw CoreException.MissingData($"Couldn't find profile {authorId}", typeof(Profile), authorId);
        var post = new Post(_options);
        var note = new Note
        {
            Text = contentSource,
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
            var parent = await _posts.LookupPost(parentId)
                         ?? throw CoreException.MissingData($"Couldn't find post {parentId} to reply to", typeof(Post),
                             parentId);
            post.InReplyTo = parent;
            post.Thread = parent.Thread;
            post.Thread.Posts.Add(post);
        }

        _posts.Add(post);
        await _posts.Commit();
        _postEvents.Created(post);
        
        return post;
    }

    public async Task<Post> Update(Post post)
    {
        // TODO: authz
        // I think authz can be conveyed around the app with just a list of claims, as long as one of the claims is
        // a profile, right?
        var previous = await _posts.LookupPost(post.Id)
                       ?? throw CoreException.MissingData($"Could not find existing post {post.Id} to update",
                           typeof(Post), post.Id);
        
        previous.Client = post.Client; // probably should come from an authz claim
        previous.InReplyTo = post.InReplyTo;
        previous.Audience = post.Audience;
        
        // remove all the removed contents, and add/update everything else
        var removed = previous.Contents.Except(post.Contents).ToArray();
        _posts.RemoveRange(removed);
        previous.Contents = post.Contents;
        
        var published = previous.PublishedDate != null;
        if (published)
        {
            previous.UpdatedDate = DateTimeOffset.Now;
            // publish again, tbd
        }
        else previous.CreatedDate = DateTimeOffset.Now;


        _posts.Update(previous);
        await _posts.Commit();
        _postEvents.Updated(previous);

        return previous;
    }

    public async Task Delete(Uuid7 id)
    {
        var post = await _posts.LookupPost(id);
        if (post is null) return;
        // TODO: authz and thread root
        post.DeletedDate = DateTimeOffset.Now;
        _posts.Remove(post);
        await _posts.Commit();
        _postEvents.Deleted(post);
    }

    public async Task<Post> Publish(Uuid7 id, bool localOnly = false)
    {
        var post = await _posts.LookupPost(id);
        if (post is null) throw CoreException.MissingData<Post>($"Can't find post {id} to publish", id);
        if (post.PublishedDate is not null)
            throw CoreException.Duplicate($"Tried to publish post {id} that is already published", id);
        post.PublishedDate = DateTimeOffset.Now;
        post.CreatedDate = DateTimeOffset.Now;
        
        _posts.Update(post);
        await _posts.Commit();
        _postEvents.Published(post);
        return post;
    }

    public async Task<Post> AddContent(Uuid7 postId, Content content)
    {
        var post = await _posts.LookupPost(postId) 
                   ?? throw CoreException.MissingData<Post>("Can't find existing post to add content", postId);
        if (!post.FediId.HasLocalAuthority(_options))
            throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
        post.Contents.Add(content);
        
        if (post.PublishedDate is not null)
        {
            post.UpdatedDate = DateTimeOffset.Now;
            _postEvents.Published(post);
        }
        
        _posts.Update(post);
        await _posts.Commit();
        _postEvents.Updated(post);
        return post;
    }

    public async Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId)
    {
        var post = await _posts.LookupPost(postId) 
                   ?? throw CoreException.MissingData<Post>("Can't find existing post to remove content", postId);
        if (!post.FediId.HasLocalAuthority(_options))
            throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
        var content = post.Contents.FirstOrDefault(c => c.Id == contentId);
        if (content is null)
        {
            _logger.LogWarning("Tried to remove content {ContentId} that doesn't exist from post {PostId}", contentId, postId);
            return post;
        }

        post.Contents.Remove(content);
        
        if (post.PublishedDate is not null)
        {
            post.UpdatedDate = DateTimeOffset.Now;
            _postEvents.Published(post);
        }
        
        _posts.Remove(content);
        _posts.Update(post);
        await _posts.Commit();
        _postEvents.Updated(post);
        return post;
    }

    public async Task<Post> UpdateContent(Uuid7 postId, Content content)
    {
        var post = await _posts.LookupPost(postId) 
                   ?? throw CoreException.MissingData<Post>("Can't find existing post to add content", postId);
        if (!post.FediId.HasLocalAuthority(_options))
            throw CoreException.WrongAuthority("Can't modify contents of remote post", post.FediId);
        post.Contents.Remove(content);
        post.Contents.Add(content);
        
        if (post.PublishedDate is not null)
        {
            post.UpdatedDate = DateTimeOffset.Now;
            _postEvents.Published(post);
        }
        
        _posts.Update(post);
        await _posts.Commit();
        _postEvents.Updated(post);
        return post;
    }
    
    public async Task<Post> ReceiveCreate(Post post)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveUpdate(Post post)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveUpdate(Uri post)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveDelete(Uri post)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveAnnounce(Post post, Uri announcedBy)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveAnnounce(Uri post, Uri announcedBy)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveUndoAnnounce(Uri post, Uri likedBy)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveLike(Uri post, Uri likedBy)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> ReceiveUndoLike(Uri post, Uri likedBy)
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
    
    private async Task<Profile?> ResolveProfile(Uri profileId,
        Profile? onBehalfOf = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        // TODO: Authz
        var profile = await _posts.LookupProfile(profileId);
        if (profile != null
            && !profile.HasLocalAuthority(_options)
            && profile.Updated.Add(TimeSpan.FromHours(12)) >= DateTime.UtcNow) return profile;

        try
        {
            if (profile != null)
            {
                var fetched = await _apClient.As(onBehalfOf).Fetch<Profile>(profileId);
                profile.ShallowCopy(fetched);
                _posts.Update(profile);
            }
            else
            {
                profile = await _apClient.As(onBehalfOf).Fetch<Profile>(profileId);
                _posts.Add(profile);
            }
            _logger.LogInformation("Fetched Profile {ProfileId} from origin", profileId);
        }
        catch (AdapterException ex)
        {
            _logger.LogError(ex, "Cannot resolve Profile {ProfileId}", profileId);
        }
        
        return profile;
    }
    
    private async Task<Post?> ResolvePost(Uri postId,
        Profile? onBehalfOf = null,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var knownPost = false;
        // TODO: Authz
        if (await _posts.LookupPost(postId) is { } post)
        {
            if (post.HasLocalAuthority(_options)) return post;
            if (post.LastSeenDate >= DateTimeOffset.Now - TimeSpan.FromMinutes(10)) return post;
            knownPost = true;
        }

        if (postId.HasLocalAuthority(_options))
        {
            _logger.LogWarning("Tried to lookup local post {PostId} that doesn't exist", postId);
            return default;
        }

        try
        {
            post = await _apClient.As(onBehalfOf).Fetch<Post>(postId);
            if (knownPost) _posts.Update(post);
            else _posts.Add(post);
            return post;
        }
        catch (AdapterException e)
        {
            _logger.LogWarning(e, "Cannot resolve post {Post}", postId);
            return default;
        }
    }
}