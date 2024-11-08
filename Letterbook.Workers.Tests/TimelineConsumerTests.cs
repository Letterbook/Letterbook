using AutoMapper;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Tests.Fakes;
using MassTransit;
using MassTransit.Testing;
using Medo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Workers.Tests;

public sealed class TimelineConsumerTests : WithMocks, IAsyncDisposable
{
	private readonly Post _post;
	private readonly ITestHarness _harness;
	private readonly ServiceProvider _provider;
	private FaultObserver _faultObserver;
	private readonly Mapper _mapper;
	private readonly ITestOutputHelper _output;

	public TimelineConsumerTests(ITestOutputHelper output)
	{
		_output = output;
		_provider = new ServiceCollection()
			.AddSingleton<ILoggerFactory>(_ => output.BuildLoggerFactory(LogLevel.Error))
			.AddMocks(this)
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddConsumer<FakeConsumer>();
				bus.AddConsumer<TimelineConsumer>();
				bus.AddTestBus();
			})
			.BuildServiceProvider(true);
		_harness = _provider.GetRequiredService<ITestHarness>();
		_faultObserver = new FaultObserver();
		_harness.Bus.ConnectReceiveObserver(_faultObserver);
		_mapper = new Mapper(_provider.GetRequiredService<MappingConfigProvider>().Posts);
		MockAuthorizeAllowAll();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_post = new FakePost("letterbook.example").Generate();
		_harness.Start().Wait();
	}

	[Fact(DisplayName = "Service bus should be configured")]
	public async Task Exists()
	{
		await _harness.Bus.Publish<FakeEvent>(new FakeEvent
		{
			Data = nameof(Exists)
		});

		Assert.True(await _harness.Published.Any<FakeEvent>());
		Assert.True(await _harness.Consumed.Any<FakeEvent>());
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should not update timeline for Created events")]
	public async Task CanConsumeCreated()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			Type = "Created",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should add to timeline for Published events")]
	public async Task CanConsumePublished()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			Type = "Published",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		AuthzTimelineServiceMock.Verify(m => m.HandlePublish(It.IsAny<Post>()));
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should update timeline for Updated events")]
	public async Task CanConsumeUpdated()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(1));
		var prev = _mapper.Map<PostDto>(_post);
		prev.Preview = $"Preview updated in {nameof(CanConsumeUpdated)}";
		prev.UpdatedDate = DateTimeOffset.UtcNow;
		prev.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			PrevData = prev,
			Type = "Updated",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		AuthzTimelineServiceMock.Verify(m => m.HandleUpdate(It.IsAny<Post>(), It.IsAny<Post>()));
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should not update timeline for malformed Updated events")]
	public async Task CanConsumeBadUpdated()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			Type = "Updated",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should add to timeline for Shared events")]
	public async Task CanConsumeShared()
	{
		var sharedBy = new FakeProfile().Generate();
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(sharedBy.Id, It.IsAny<ProfileId?>())).ReturnsAsync(sharedBy);
		_post.PublishedDate = DateTimeOffset.UtcNow;

		_output.WriteLine("sharedBy: {0}", sharedBy.GetId());
		var message = new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			Type = "Shared",
			Sender = sharedBy.GetId()
		};
		_output.WriteLine("message.Sender: {0}", message.Sender);
		await _harness.Bus.Publish<PostEvent>(message);

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		Assert.Empty(_faultObserver.Faults);
		AuthzTimelineServiceMock.Verify(m => m.HandleShare(It.IsAny<Post>(), sharedBy));
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should not update timeline for Liked events")]
	public async Task CanConsumeLiked()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _mapper.Map<PostDto>(_post),
			Type = "Liked",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		AuthzTimelineServiceMock.VerifyNoOtherCalls();
		Assert.Empty(_faultObserver.Faults);
	}

	/*
	 * Support methods
	 */

	public async ValueTask DisposeAsync()
	{
		await _provider.DisposeAsync();
		await _harness.Stop();
	}
}