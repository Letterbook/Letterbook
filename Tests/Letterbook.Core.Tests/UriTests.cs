using Letterbook.Core.Extensions;

namespace Letterbook.Core.Tests;

public class UriTests
{
	[InlineData("subdomain.website.example:9999")]
	[InlineData("https://subdomain.website.example:9999")]
	[InlineData("https://subdomain.website.example:9999/whatever")]
	[InlineData("subdomain.website.example")]
	[Theory]
	public void CanParseDomain(string domain)
	{
		Assert.True(UriExtensions.TryParseDomain(domain, out var uri));
		Assert.NotEqual("", uri.Host);
	}
}