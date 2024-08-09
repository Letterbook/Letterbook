using System.Net.Mime;

namespace Letterbook.Core.Models;

/// <summary>
/// A short, single page, textual post content. Corresponds to AS:NoteObject.
/// Posts that include a Note should usually serialize as a Note in AP documents
/// </summary>
public class Note : Content
{
	public string? SourceText { get; set; }
	public ContentType? SourceContentType { get; set; }
	public override string Type => "Note";

	public override string? GeneratePreview()
	{
		// TODO: implement this in a less naive way
		// Something like first paragraph or x words/characters
		Preview = SourceText?[..Math.Min(SourceText.Length, 100)];
		return Preview;
	}

	// TODO: proper sanitization
	public override void Sanitize()
	{
		Html = SourceText ?? "";
	}

	public override void UpdateFrom(Content content)
	{
		if (content is not Note note)
			throw new ArgumentException($"{content.Type} is not {Type}", nameof(content));

		SourceText = note.SourceText;
		base.UpdateFrom(note);
	}
}