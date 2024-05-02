using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.IntegrationTests;

// These tests bypass the API (which mostly doesn't exist yet), and mock the database (because managing test data is
// hard). Everything else is live. So, the end result is they send real federated messages using the real core logic
// and AP client. The test data is entirely fabricated by the test, and we don't have to worry about our own
// authentication.
[Trait("ActivityPub", "Client")]
[Trait("Composite", "ActivityPub")]
[Trait("Composite", "Mastodon")]
public class ActivityPubClientCompositeTests : IClassFixture<HostFixture>
{
	private readonly ITestOutputHelper _output;
	private readonly HostFixture _hostFactory;
	private FakeProfile _fakeProfile;
	private readonly IOptions<CoreOptions> _options;

	public ActivityPubClientCompositeTests(ITestOutputHelper output, HostFixture hostFactory)
	{
		_output = output;
		_hostFactory = hostFactory;
		_options = _hostFactory.Services.GetService<IOptions<CoreOptions>>();
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
		Assert.NotNull(_hostFactory);
	}

	[Fact(Skip = "Requires actor controllers")]
	public async Task SendFollow()
	{
		var remote = new Uri("http://localhost:3080/users/user");
		var profile = _fakeProfile.Generate();
		var target = new FakeProfile(remote).Generate();
		HostFixture.Mocks.AccountProfileMock.Setup(m => m.LookupProfile((Guid)profile.Id!))
			.ReturnsAsync(profile);
		// HostFixture.Mocks.AccountProfileMock.Setup(m => m.LookupProfile(remote))
		// .ReturnsAsync(target);

		using var scope = _hostFactory.Services.CreateScope();

		var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

		var result = await profileService.As(Enumerable.Empty<Claim>()).Follow((Guid)profile.Id!, remote);
	}
}