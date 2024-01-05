using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Letterbook.Core.Models;

/// <summary>
/// A short, single page, textual post content. Corresponds to AS NoteObject.
/// Posts that include a Note should usually serialize as a Note in AP documents
/// </summary>
public class Note : Content
{
    public override string Type => "Note";

    public required string Content { get; set; }
    
    public Note()
    {}

    public Note(Post post, Uri idUri, string content)
    {
        IdUri = idUri;
        Content = content;
        Post = post;
        Post.Contents.Add(this);
    }
}