using System.Net.Mime;
using System.Web;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public class TextSanitizer : IContentSanitizer
{
	public string Sanitize(string content, string baseUrl = "") => HttpUtility.HtmlEncode(content);
	public ContentType ContentType { get; } = new(Content.PlainTextMediaType);
	public ContentType Result { get; } = new(Content.HtmlMediaType);
}