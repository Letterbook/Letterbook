using Bogus;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Letterbook.Core.Tests;


public class ActivityTest : WithMocks
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly ActivityService _activityService;
    private readonly Mock<ILogger<ActivityService>> _loggerMock;
    private int _randomSeed;
    private FakeNote _fakeNote;
    private FakeProfile _fakeProfile;

    public ActivityTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _randomSeed = new Random().Next();
        _loggerMock = new Mock<ILogger<ActivityService>>();
        _activityService = new ActivityService(ActivityAdapterMock.Object, _loggerMock.Object,
            Mock.Of<IActivityEventService>());

        Randomizer.Seed = new Random(_randomSeed);
        _outputHelper.WriteLine($"Bogus random seed: {_randomSeed}");

        _fakeNote = new FakeNote();
        _fakeProfile = new FakeProfile();
    }

    [Fact(DisplayName = "Should create notes")]
    public async void TestCreateNotes()
    {
        var note = Enumerable.Range(0, 1).Select(_ => _fakeNote.Generate());
        await _activityService.ReceiveNotes(note, ActivityType.Create, _fakeProfile.Generate());

        ActivityAdapterMock.Verify(mock => mock.RecordNotes(It.IsAny<IEnumerable<Note>>()));
    }

    [Fact(DisplayName = "Should add Likes to preexisting notes")]
    public async void TestLikeFoundNote()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();
        ActivityAdapterMock.Setup(m => m.LookupNoteUrl(note.Id.ToString())).Returns(note);

        await _activityService.ReceiveNotes(notes, ActivityType.Like, actor);

        Assert.Contains(actor, notes.First().LikedBy);
    }

    [Fact(DisplayName = "Should not record new notes on like")]
    public async void TestLikeNotFoundNote()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();

        var created = await _activityService.ReceiveNotes(notes, ActivityType.Like, actor);

        Assert.DoesNotContain(actor, notes.First().LikedBy);
        Assert.False(created);
    }

    [Fact(DisplayName = "Should delete notes")]
    public async void TestDeleteNote()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();
        note.Creators.Add(actor);
        ActivityAdapterMock.Setup(m => m.DeleteNotes(It.IsAny<IEnumerable<Note>>())).Returns(true);

        var deleted = await _activityService.ReceiveNotes(notes, ActivityType.Delete, actor);

        Assert.True(deleted);
    }

    [Fact(DisplayName = "Should do nothing when there is no note to delete")]
    public async void TestDeleteNoteNotFound()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();
        note.Creators.Add(actor);

        var deleted = await _activityService.ReceiveNotes(notes, ActivityType.Delete, actor);

        Assert.False(deleted);
    }

    [Fact(DisplayName = "Should remove likes on dislike")]
    public async void TestDislike()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();
        note.LikedBy.Add(actor);
        ActivityAdapterMock.Setup(m => m.LookupNoteUrl(It.IsAny<string>())).Returns(note);

        var done = await _activityService.ReceiveNotes(notes, ActivityType.Dislike, actor);

        Assert.True(done);
        Assert.DoesNotContain(actor, note.LikedBy);
    }

    [Fact(DisplayName = "Should do nothing on dislike when note not found")]
    public async void TestDislikeNotFound()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();

        var done = await _activityService.ReceiveNotes(notes, ActivityType.Dislike, actor);

        Assert.False(done);
    }

    [Fact(DisplayName = "Should do nothing on dislike when not liked by actor")]
    public async void TestDislikeNotLiked()
    {
        var note = _fakeNote.Generate();
        var notes = new List<Note> { note };
        var actor = _fakeProfile.Generate();
        ActivityAdapterMock.Setup(m => m.LookupNoteUrl(It.IsAny<string>())).Returns(note);

        var done = await _activityService.ReceiveNotes(notes, ActivityType.Dislike, actor);

        Assert.False(done);
    }
}