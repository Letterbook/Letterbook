using Letterbook.IntegrationTests.Fixtures;

namespace Letterbook.IntegrationTests;

// Future Infra: MessageQueue, ObjectStore, Email, BulkEmail, Cache, Backplane
[Trait("Infra", "Postgres")]
[Trait("Infra", "Timescale")]
[Trait("Driver", "Api")]
public class TimelinesApiTests : IClassFixture<HostFixture<TimelinesApiTests>>, ITestSeed
{
	private readonly HostFixture<TimelinesApiTests> _host;
	private readonly HttpClient _client;
	static int? ITestSeed.Seed() => null;
	static int ITestSeed.TimelineCount() => 500;

	public TimelinesApiTests(HostFixture<TimelinesApiTests> host)
	{
		_host = host;
		_client = _host.CreateClient();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
		Assert.NotNull(_client);
	}
}