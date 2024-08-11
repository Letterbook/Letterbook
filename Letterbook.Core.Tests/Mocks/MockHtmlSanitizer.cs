using System.Net.Mime;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Mocks;

public class MockHtmlSanitizer : IContentSanitizer
{
	public string Sanitize(string content, string baseUrl = "") => content;
	public ContentType ContentType { get; } = new(Content.HtmlMediaType);
}

public class MockTextSanitizer : IContentSanitizer
{
	public string Sanitize(string content, string baseUrl = "") => content;
	public ContentType ContentType { get; } = new(Content.PlainTextMediaType);
}