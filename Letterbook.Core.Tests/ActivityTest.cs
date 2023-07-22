using Bogus;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class ActivityTest : WithMocks
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly ActivityService _activityService;
    private readonly Mock<ILogger<ActivityService>> _logger;
    private int _randomSeed;
    private FakeNote _fakeNote;
    private FakeProfile _fakeProfile;

    public ActivityTest(ITestOutputHelper  outputHelper)
    {
        _outputHelper = outputHelper;
        _randomSeed = new Random().Next();
        _logger = new Mock<ILogger<ActivityService>>();
        _activityService = new ActivityService(FediAdapter.Object, ActivityAdapter.Object, _logger.Object);

        Randomizer.Seed = new Random(_randomSeed);
        _outputHelper.WriteLine($"Bogus random seed: {_randomSeed}");

        _fakeNote = new FakeNote();
        _fakeProfile = new FakeProfile();
    }

    [Fact(DisplayName = "Create method should exist")]
    public void TestCreateExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Create());
    }

    [Fact]
    public async void TestReceiveNotes()
    {
        var note = Enumerable.Range(0, 1) .Select(_ => _fakeNote.Generate());
        await _activityService.ReceiveNotes(note, ActivityType.Create, _fakeProfile.Generate());
        
        ActivityAdapter.Verify(mock => mock.RecordNotes(It.IsAny<IEnumerable<Note>>()));
    }

    [Fact(DisplayName = "Deliver method should exist")]
    public void TestDeliverExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Deliver(null!));
    }
}