using JetBrains.Annotations;

namespace Letterbook.Core.Models;

/// <summary>
/// A short, single page, textual post content. Corresponds to AS NoteObject.
/// Posts that include a Note should usually serialize as a Note in AP documents
/// </summary>
public class Note : IContent
{
    [UsedImplicitly]
    private Note()
    {
        Uri = default!;
        Post = default!;
        Content = default!;
    }
    
    public Guid Id { get; set; }
    public Uri Uri { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public string Type => "Note";
    
    public Post Post { get; set; }
    public string Content { get; set; }

    public Note(Post post, string content) : this(post, post.Uri, content)
    {
        Post = post;
        Content = content;
    }
    
    public Note(Post post, Uri uri, string content)
    {
        Id = Guid.NewGuid();
        Uri = uri;
        Content = content;
        Post = post;
    }
}