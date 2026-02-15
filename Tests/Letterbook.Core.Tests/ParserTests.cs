using Letterbook.Core.Extensions;

namespace Letterbook.Core.Tests;

public class ParserTests
{
	public record CanParseFact(string Given, string? Handle, string? Host, bool Success);

	public class CanParseFacts : Xunit.TheoryData<CanParseFact>
	{
		public CanParseFacts()
		{
			Add(new CanParseFact("@someone@peer.example", "someone", "https://peer.example/", true));
			Add(new CanParseFact("@someone@peer.example:5127", null, null, false));
			Add(new CanParseFact("someone@peer.example", "someone", "https://peer.example/", true));
			Add(new CanParseFact("acct:someone@peer.example", "someone", "https://peer.example/", true));
			Add(new CanParseFact("acct://someone@peer.example", "someone", "https://peer.example/", true));
			Add(new CanParseFact("https://someone@peer.example", "someone", "https://peer.example/", true));
			Add(new CanParseFact("https://peer.example/someone", null, null, false));
			Add(new CanParseFact("https://peer.example/@someone", null, null, false));
			Add(new CanParseFact("@someone", null, null, false));
			Add(new CanParseFact("someone@localhost", null, null, false));
			Add(new CanParseFact("someone@127.0.0.1", null, null, false));
			Add(new CanParseFact("someone@192.168.0.1", null, null, false));
			Add(new CanParseFact("some other string", null, null, false));
			Add(new CanParseFact("a string that @resembles a handle", null, null, false));
			Add(new CanParseFact("a string that acct:resembles a handle", null, null, false));
		}
	}

	[ClassData(typeof(CanParseFacts))]
	[Theory(DisplayName = "Should parse query terms")]
	public void CanParse(CanParseFact fact)
	{
		var success = UriExtensions.TryParseHandle(fact.Given, out var handle, out var host);
		Assert.Equal(fact.Success, success);
		Assert.Equal(fact.Handle, handle);
		Assert.Equal(fact.Host, host?.ToString());
	}
}