using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public sealed class PostEventPublisherTests : WithMocks, IAsyncDisposable
{
	private readonly ServiceProvider _provider;
	private readonly IPostEventPublisher _publisher;
	private readonly ITestHarness _harness;
	private readonly Profile _profile;
	private readonly Post _post;

	public PostEventPublisherTests(ITestOutputHelper output)
	{
		_provider = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<IPostEventPublisher, PostEventPublisher>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddTestBus();
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IPostEventPublisher>();
		_harness = _provider.GetRequiredService<ITestHarness>();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_profile = new FakeProfile("letterbook.example").Generate();
		_post = new FakePost(_profile).Generate();

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
	}

	[Fact(DisplayName = "Should publish Created events")]
	public async Task CanPublishCreated()
	{
		await _harness.Start();
		await _publisher.Created(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Created"));
	}

	[Fact(DisplayName = "Should publish Deleted events")]
	public async Task CanPublishDeleted()
	{
		await _harness.Start();
		await _publisher.Deleted(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Deleted"));
	}

	[Fact(DisplayName = "Should publish Updated events")]
	public async Task CanPublishUpdated()
	{
		await _harness.Start();
		await _publisher.Updated(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Updated"));
	}

	[Fact(DisplayName = "Should publish Published events")]
	public async Task CanPublishPublished()
	{
		await _harness.Start();
		await _publisher.Published(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Published"));
	}

	[Fact(DisplayName = "Should publish Liked events")]
	public async Task CanPublishLiked()
	{
		await _harness.Start();
		await _publisher.Liked(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Liked"));
	}

	[Fact(DisplayName = "Should publish Shared events")]
	public async Task CanPublishShared()
	{
		await _harness.Start();
		await _publisher.Shared(_post, _profile.GetId(), []);

		Assert.True(await _harness.Published.Any<PostEvent>(msg => msg.Context.Message.Type == "Shared"));
	}

	public async ValueTask DisposeAsync()
	{
		await _provider.DisposeAsync();
	}
}