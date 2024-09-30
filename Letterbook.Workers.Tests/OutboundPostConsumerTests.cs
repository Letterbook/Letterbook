using ActivityPub.Types.AS;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Testing;
using Medo;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable.Moq;
using Moq;

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
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IPostEventPublisher>();
		_harness = _provider.GetRequiredService<ITestHarness>();
		_profile = new FakeProfile("letterbook.example").Generate();
		_post = new FakePost(_profile).Generate();

		_harness.Start().Wait();

		MockAuthorizeAllowAll();
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
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(_profile.GetId(), It.IsAny<Uuid7?>()))
			.ReturnsAsync(_profile);

		await _publisher.Published(_post, _profile.GetId(), []);

		Assert.True(await _harness.Consumed.Any<PostEvent>());
		Assert.Empty(_harness.Consumed.Select<PostEvent>().AsEnumerable().Select(m => m.Exception).WhereNotNull());
		ActivityPublisherMock.Verify(m => m.Deliver(follower.Inbox, It.IsAny<ASType>(), It.Is<Profile>(profile => profile.GetId() == _profile.GetId())));
	}
}