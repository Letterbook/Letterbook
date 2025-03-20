using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Infra", "Federation")]
[Trait("Driver", "Api")]
public class ActivityPubClientCompositeTests : IClassFixture<HostFixture<ActivityPubClientCompositeTests>>, ITestSeed
{
	private readonly ITestOutputHelper _output;
	private readonly HostFixture<ActivityPubClientCompositeTests> _host;
	private FakeProfile _fakeProfile;
	private readonly IOptions<CoreOptions> _options;
	public static int? Seed() => null;

	public ActivityPubClientCompositeTests(ITestOutputHelper output, HostFixture<ActivityPubClientCompositeTests> host)
	{
		_output = output;
		_host = host;
		_options = _host.Services.GetRequiredService<IOptions<CoreOptions>>();
		_output.WriteLine(_options.Value.DomainName);

		// Initialize with a consistent seed, so we get consistent data.
		// This might not actually be necessary, or even desirable. It's hard to think through in the abstract.
		// We can change it when federation actually works, if needed.
		_output.WriteLine($"Bogus seed: {Init.WithSeed(99263675)}");
		_fakeProfile = new FakeProfile(_options.Value.DomainName);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(Skip = "Requires federation infra")]
	public async Task SendFollow()
	{
		var remote = new Uri("http://localhost:3080/users/user");
		var profile = _fakeProfile.Generate();
		var target = new FakeProfile(remote).Generate();

		using var scope = _host.Services.CreateScope();

		var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

		var result = await profileService.As(Enumerable.Empty<Claim>()).Follow(profile.Id, remote);
	}
}