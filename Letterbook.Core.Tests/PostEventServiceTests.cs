using System.Reactive;
using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class PostEventServiceTests : WithMocks
{
	private PostEventService _service;
	private readonly FakeProfile _fakeProfile;
	private readonly Profile _profile;
	private readonly FakePost _fakePost;
	private readonly Post _post;
	private readonly Subject<CloudEvent> _subject;

	public PostEventServiceTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_subject = new Subject<CloudEvent>();
		MessageBusAdapterMock.Setup(m => m.OpenChannel<It.IsAnyType>(It.IsAny<string?>()))
			.Returns(_subject.AsObserver());
		_service = new PostEventService(Mock.Of<ILogger<PostEventService>>(), CoreOptionsMock, MessageBusAdapterMock.Object);

		_fakeProfile = new FakeProfile(CoreOptionsMock.Value.BaseUri().Authority);
		_profile = _fakeProfile.Generate();
		_fakePost = new FakePost(_profile, opts: CoreOptionsMock.Value);
		_post = _fakePost.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
	}

	[Fact(DisplayName = "Should emit Created event")]
	public void CanEmitCreated()
	{
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Created", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(_post, actual.Post);
		});

		_service.Created(_post);
	}

	[Fact(DisplayName = "Should emit Updated event")]
	public void CanEmitUpdated()
	{
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Updated", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(_post, actual.Post);
		});

		_service.Updated(_post);
	}

	[Fact(DisplayName = "Should emit Deleted event")]
	public void CanEmitDeleted()
	{
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Deleted", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(_post, actual.Post);
		});

		_service.Deleted(_post);
	}

	[Fact(DisplayName = "Should emit Published event")]
	public void CanEmitPublished()
	{
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Published", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(_post, actual.Post);
		});

		_service.Published(_post);
	}

	[Fact(DisplayName = "Should emit Liked event")]
	public void CanEmitLiked()
	{
		var actor = _fakeProfile.Generate();
		var expected = new IPostEvents.Data { Post = _post, ProfileId = actor.GetId25() };
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Liked", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(expected.ProfileId, actual.ProfileId);
			Assert.Equal(expected.Post, actual.Post);
		});

		_service.Liked(_post, actor);
	}

	[Fact(DisplayName = "Should emit Shared event")]
	public void CanEmitShared()
	{
		var actor = _fakeProfile.Generate();
		var expected = new IPostEvents.Data { Post = _post, ProfileId = actor.GetId25() };
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Shared", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(expected.ProfileId, actual.ProfileId);
			Assert.Equal(expected.Post, actual.Post);
		});

		_service.Shared(_post, actor);
	}

	[Fact(DisplayName = "Should emit Received event")]
	public void CanEmitReceived()
	{
		var actor = _fakeProfile.Generate();
		var expected = new IPostEvents.Data { Post = _post, ProfileId = actor.GetId25() };
		_subject.Subscribe(c =>
		{
			var action = c.Type!.Split(".").Last();
			Assert.Equal("Received", action);
			var actual = Assert.IsType<IPostEvents.Data>(c.Data);
			Assert.Equal(expected.ProfileId, actual.ProfileId);
			Assert.Equal(expected.Post, actual.Post);
		});

		_service.Received(_post, actor);
	}
}