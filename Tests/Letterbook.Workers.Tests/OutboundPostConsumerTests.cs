using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable;
using Moq;
using Audience = Letterbook.Core.Models.Audience;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Tests;

public class OutboundPostConsumerTests : WithMocks, IAsyncDisposable
{
	public async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);
		await _provider.DisposeAsync();
	}

	private readonly ServiceProvider _provider;
	private readonly IPostEventPublisher _publisher;
	private readonly ITestHarness _harness;
	private readonly Post _post;
	private readonly Profile _profile;

	public OutboundPostConsumerTests()
	{
		_provider = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<IPostEventPublisher, PostEventPublisher>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddConsumer<OutboundPostConsumer>();
				bus.AddTestBus();
				bus.SetTestTimeouts(testTimeout: TimeSpan.FromMilliseconds(3000), testInactivityTimeout: TimeSpan.FromMilliseconds(1500));
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IPostEventPublisher>();
		_harness = _provider.GetRequiredService<ITestHarness>();
		_profile = new FakeProfile("letterbook.example").Generate();
		_post = new FakePost(_profile).Generate();

		_harness.Start().Wait();

		MockAuthorizeAllowAll();

		ProfileServiceAuthMock.Setup(m => m.LookupProfile(_profile.Id, It.IsAny<ProfileId?>()))
			.ReturnsAsync(_profile);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
	}

	[Fact(DisplayName = "Should consume post events")]
	public async Task CanConsume()
	{
		var follower = new FakeProfile("peer.example").Generate();
		follower.SharedInbox = null;
		_profile.AddFollower(follower, FollowState.Accepted);
		_profile.Headlining.FirstOrDefault(h => h.FediId == Audience.Followers(_profile).FediId)?
			.Members.Add(follower);
		DataAdapterMock.Setup(m => m.QueryFrom(_post, p => p.Audience)).Returns(_post.Audience.BuildMock());
		DataAdapterMock.Setup(m => m.QueryFrom(_post, p => p.AddressedTo)).Returns(_post.AddressedTo.BuildMock());
		DataAdapterMock.Setup(m => m.Audiences(It.IsAny<Uri[]>())).Returns(_post.Audience.BuildMock());

		await _publisher.Published(_post, _profile.GetId(), []);

		Assert.True(await _harness.Consumed.Any<PostEvent>());
		Assert.Empty(_harness.Consumed.Select<PostEvent>().AsEnumerable().Select(m => m.Exception).WhereNotNull());
		ActivityPublisherMock.Verify(m =>
			m.Publish(follower.Inbox, It.IsAny<Post>(), It.Is<Profile>(profile => profile.GetId() == _profile.GetId()), It.IsAny<IEnumerable<Claim>>(), null));
	}

	[Fact(DisplayName = "Should send on Publish")]
	public async Task CanDeliverPublish()
	{
		var recipient = new FakeProfile("https://peer.example").Generate();
		recipient.SharedInbox = null;
		var audience = Audience.Followers(_profile);
		audience.Members.Add(recipient);

		DataAdapterMock.Setup(m => m.QueryFrom(_post, p => p.AddressedTo)).Returns(new List<Mention>().BuildMock());
		DataAdapterMock.Setup(m => m.Audiences(It.IsAny<Uri[]>())).Returns(new List<Audience> { audience }.BuildMock());

		await _publisher.Published(_post, _profile.GetId(), []);

		Assert.True(await _harness.Consumed.Any<PostEvent>());
		ActivityPublisherMock.Verify(m => m.Publish(recipient.Inbox, _post, _profile, It.IsAny<IEnumerable<Claim>>(), null));
	}

	[Fact(DisplayName = "Should send on Update")]
	public async Task CanDeliverUpdate()
	{
		var recipient = new FakeProfile("https://peer.example").Generate();
		recipient.SharedInbox = null;
		var audience = Audience.Followers(_profile);
		audience.Members.Add(recipient);

		DataAdapterMock.Setup(m => m.QueryFrom(_post, p => p.AddressedTo)).Returns(new List<Mention>().BuildMock());
		DataAdapterMock.Setup(m => m.Audiences(It.IsAny<Uri[]>())).Returns(new List<Audience> { audience }.BuildMock());

		await _publisher.Updated(_post, _profile.GetId(), []);

		Assert.True(await _harness.Consumed.Any<PostEvent>());
		ActivityPublisherMock.Verify(m => m.Update(recipient.Inbox, _post, _profile, It.IsAny<IEnumerable<Claim>>(), null));
	}

	[Fact(DisplayName = "Should send on Delete")]
	public async Task CanDeliverDelete()
	{
		var recipient = new FakeProfile("https://peer.example").Generate();
		recipient.SharedInbox = null;
		var audience = Audience.Followers(_profile);
		audience.Members.Add(recipient);

		DataAdapterMock.Setup(m => m.QueryFrom(_post, p => p.AddressedTo)).Returns(new List<Mention>().BuildMock());
		DataAdapterMock.Setup(m => m.Audiences(It.IsAny<Uri[]>())).Returns(new List<Audience> { audience }.BuildMock());

		await _publisher.Deleted(_post, _profile.GetId(), []);

		Assert.True(await _harness.Consumed.Any<PostEvent>());
		ActivityPublisherMock.Verify(m => m.Delete(recipient.Inbox, _post, It.IsAny<IEnumerable<Claim>>(), _profile));
	}
}