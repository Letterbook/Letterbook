using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Mocks;
using Medo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class PostServiceTests : WithMocks
{
	private readonly ITestOutputHelper _output;
	private readonly PostService _service;
	private readonly Profile _profile;
	private readonly Post _post;
	private readonly FakeProfile _fakeProfile;

	public PostServiceTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_service = new PostService(Mock.Of<ILogger<PostService>>(), CoreOptionsMock,
			DataAdapterMock.Object, PostEventServiceMock.Object, ActivityPubClientMock.Object,
			[new MockHtmlSanitizer(), new MockTextSanitizer()]);
		_fakeProfile = new FakeProfile("letterbook.example");
		_profile = _fakeProfile.Generate();
		_post = new FakePost(_profile);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
	}

	[Fact(DisplayName = "Should create unpublished draft notes")]
	public async Task CanDraftNote()
	{
		DataAdapterMock.Setup(m => m.LookupProfile(It.IsAny<Uuid7>()))
			.ReturnsAsync(_profile);

		var actual = await _service.DraftNote(_profile.Id, "Test content");

		var expected = DateTimeOffset.Now;
		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Contents);
		Assert.Equal("Test content", actual.Contents.First().Preview);
		Assert.True((actual.CreatedDate - expected).Duration() <= TimeSpan.FromSeconds(1));
		Assert.Null(actual.PublishedDate);
		Assert.Empty(actual.Audience);
	}

	[Fact(DisplayName = "Should create unpublished reply posts")]
	public async Task CanDraftReply()
	{
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		DataAdapterMock.Setup(m => m.LookupProfile(It.IsAny<Uuid7>()))
			.ReturnsAsync(_profile);

		var actual = await _service.DraftNote(_profile.Id, "Test content", _post.Id);

		Assert.Equal(_post.Id, actual.InReplyTo?.Id);
	}

	[Fact(DisplayName = "Should update post")]
	public async Task CanUpdate()
	{
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		var update = new FakePost(_profile).Generate();
		update.Id = _post.Id;
		update.FediId = _post.FediId;
		update.Audience.Add(Audience.Public);

		var actual = await _service.Update(_post.GetId(), update);
		Assert.Contains(Audience.Public, actual.Audience);
	}

	[Fact(DisplayName = "Update should add post content")]
	public async Task UpdateCanAddContent()
	{
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		var update = new FakePost(_profile).Generate();
		update.Id = _post.Id;
		update.FediId = _post.FediId;
		var note = new Fakes.FakeNote(update).Generate();
		update.Contents.Add(note);

		var actual = await _service.Update(_post.GetId(), update);
		Assert.Equal(update.Contents.First().Summary, actual.Contents.First().Summary);
		Assert.Equal(2, update.Contents.Count);
	}

	[Fact(DisplayName = "Update should remove post content")]
	public async Task UpdateCanRemoveContent()
	{
		_post.Contents.Add(new Fakes.FakeNote(_post).Generate());
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		var update = new FakePost(_profile).Generate();
		update.Id = _post.Id;
		update.FediId = _post.FediId;

		var actual = await _service.Update(_post.GetId(), update);
		Assert.Single(actual.Contents);
		Assert.Contains(update.Contents.First(), actual.Contents);
	}

	[Fact(DisplayName = "Update should modify post content")]
	public async Task UpdateCanModifyContent()
	{
		var before = (_post.Contents.First() as Note)!;
		var expectedNote = new Fakes.FakeNote(_post).Generate();
		expectedNote.Id = before.Id;
		_post.Contents.Add(new Fakes.FakeNote(_post).Generate());
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		var update = new FakePost(_profile).Generate();
		update.Contents.Clear();
		update.Contents.Add(expectedNote);
		update.Id = _post.Id;
		update.FediId = _post.FediId;

		var actual = await _service.Update(_post.GetId(), update);
		var actualNote = Assert.IsType<Note>(actual.Contents.First());
		Assert.Equal(expectedNote.SourceText, actualNote.SourceText);
	}

	[Fact(DisplayName = "Should not update sensitive fields")]
	public async Task NoUpdateCreators()
	{
		var evilProfile = new FakeProfile("letterbook.example").Generate();
		var evilDomain = new UriBuilder(_post.FediId);
		evilDomain.Host = "letterbook.evil";
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);
		var update = new FakePost(evilProfile).Generate();
		update.Id = _post.Id;
		update.FediId = evilDomain.Uri;

		await Assert.ThrowsAsync<CoreException>(() => _service.Update(_post.GetId(), update));
	}

	[Fact(DisplayName = "Should delete posts")]
	public async Task CanDelete()
	{
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);

		await _service.Delete(_post.Id);

		DataAdapterMock.Verify(m => m.Remove(_post));
	}

	[Fact(DisplayName = "Should publish drafted posts")]
	public async Task CanPublish()
	{
		_post.PublishedDate = null;
		var expected = DateTimeOffset.Now;
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);

		var actual = await _service.Publish(_post.Id);

		var published = Assert.NotNull(actual.PublishedDate);
		Assert.InRange(published, expected, DateTimeOffset.Now);
	}

	[Fact(DisplayName = "Should add new content to a post")]
	public async Task CanAddContent()
	{
		var content = new Fakes.FakeNote(_post).Generate();
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);

		var actual = await _service.AddContent(_post.Id, content);

		Assert.Contains(content, actual.Contents);
	}

	[Fact(DisplayName = "Should update content in a post")]
	public async Task CanUpdateContent()
	{
		var expected = "test content";
		var note = new Fakes.FakeNote(_post).Generate();
		note.Id = _post.Contents.First().Id;
		note.FediId = _post.Contents.First().FediId;
		note.SourceText = expected;
		note.GeneratePreview();
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);

		var result = await _service.UpdateContent(_post.Id, note.Id, note);

		var actual = Assert.IsType<Note>(result.Contents.FirstOrDefault());
		Assert.Equal(expected, actual.SourceText);
	}

	[Fact(DisplayName = "Should remove content from a post")]
	public async Task CanRemoveContent()
	{
		var content = new Fakes.FakeNote(_post).Generate();
		_post.Contents.Add(content);
		DataAdapterMock.Setup(m => m.LookupPost(_post.Id)).ReturnsAsync(_post);

		var actual = await _service.RemoveContent(_post.Id, content.Id);

		Assert.DoesNotContain(content, actual.Contents);
	}
}