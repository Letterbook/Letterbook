using System.Text.Json;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Mappers.Converters;
using Medo;

namespace Letterbook.Core.Tests;

public class ProfileIdTests
{
	private JsonSerializerOptions _opts = new();

	public ProfileIdTests()
	{
		_opts.Converters.Add(new ProfileIdJsonConverter());
		_opts.Converters.Add(new Uuid7JsonConverter());
	}

	[Fact]
	public void TestJson()
	{
		var expected = Uuid7.NewUuid7();
		var id = new ProfileId(expected);
		var actual = JsonSerializer.Serialize(id, _opts);

		Assert.Equal($"\"{expected.ToId25String()}\"", actual);
	}
}