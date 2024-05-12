using System.Reactive;
using System.Reactive.Subjects;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class ActivityMessageServiceTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
	private readonly ITestOutputHelper _output;
	private readonly IJsonLdSerializer _serializer;
	private ActivityMessageService _service;
	private readonly Profile _profile;
	private readonly Uri _inbox;
	private readonly AnnounceActivity _activity;
	private readonly Subject<CloudEvent> _subject;

	public ActivityMessageServiceTests(ITestOutputHelper output, JsonLdSerializerFixture serializer)
	{
		_output = output;
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_serializer = serializer.JsonLdSerializer;

		var faker = new FakeProfile("letterbook.example");
		_profile = faker.Generate();
		_inbox = new Uri("https://peer.example/actor/someactor/inbox");
		_activity = new AnnounceActivity();
		_activity.Actor.Add(_profile.FediId);
		_activity.Object.Add(new Uri("https://peer.example/object/12345"));
		_subject = new Subject<CloudEvent>();

		_service = new ActivityMessageService(Mock.Of<ILogger<ActivityMessageService>>(), CoreOptionsMock, _serializer, MessageBusAdapterMock.Object);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
	}

	[Fact(DisplayName = "Should send a message to the delivery queue")]
	public void CanDeliver()
	{
		_subject.Subscribe(c =>
		{
			var type = c.Type!.Split(".").Last();
			Assert.Equal("AnnounceActivity", type);
		});
		_service.Deliver(_inbox, _activity, _profile);
	}
}