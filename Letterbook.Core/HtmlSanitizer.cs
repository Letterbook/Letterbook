using System.Net.Mime;
using Ganss.Xss;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public class HtmlSanitizer(IHtmlSanitizer sanitizer) : IContentSanitizer
{
	public string Sanitize(string content, string baseUrl = "") => sanitizer.Sanitize(content, baseUrl);
	public ContentType ContentType { get; } = new(Content.HtmlMediaType);
	public ContentType Result { get; } = new(Content.HtmlMediaType);
}