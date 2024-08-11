using System.Net.Mime;
using Letterbook.Core.Exceptions;

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

	public override void Sanitize(IEnumerable<IContentSanitizer> sanitizers)
	{
		var (text, type) = SourceText != null && SourceContentType != null ? (SourceText, SourceContentType) : (Html, ContentType);
		var sanitizer = sanitizers.FirstOrDefault(s => s.ContentType.MediaType == type.MediaType)
		                ?? throw CoreException.InvalidRequest("Unknown media type", "ContentType", ContentType?.MediaType ?? "unknown");
		Html = sanitizer.Sanitize(text, FediId.Authority);
		ContentType = sanitizer.Result;
	}

	public override void UpdateFrom(Content content)
	{
		if (content is not Note note)
			throw new ArgumentException($"{content.Type} is not {Type}", nameof(content));

		SourceText = note.SourceText;
		base.UpdateFrom(note);
	}
}