namespace Letterbook.Core.Models;

/// <summary>
/// A short, single page, textual post content. Corresponds to AS:NoteObject.
/// Posts that include a Note should usually serialize as a Note in AP documents
/// </summary>
public class Note : Content
{
    public required string Content { get; set; }
    public override string Type => "Note";

    public override string? GeneratePreview()
    {
        // TODO: implement this in a less naive way
        // Something like first paragraph or x words/characters
        Preview = Content[..Math.Min(Content.Length, 100)];
        return Preview;
    }

    public override void Sanitize()
    {
        throw new NotImplementedException();
    }
}