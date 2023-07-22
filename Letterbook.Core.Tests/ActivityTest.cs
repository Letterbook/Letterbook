using Bogus;
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

    public ActivityTest(ITestOutputHelper  outputHelper)
    {
        _outputHelper = outputHelper;
        _randomSeed = new Random().Next();
        _logger = new Mock<ILogger<ActivityService>>();
        _activityService = new ActivityService(FediAdapter.Object, ActivityAdapter.Object, _logger.Object);

        Randomizer.Seed = new Random(_randomSeed);
        _outputHelper.WriteLine($"Bogus random seed: {_randomSeed}");

    }

    [Fact(DisplayName = "Create method should exist")]
    public void TestCreateExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Create());
    }

    [Fact]
    public void TestReceiveNotes()
    {
        var note = new FakeNote().Generate();
        Assert.NotNull(note);
    }

    [Fact(DisplayName = "Deliver method should exist")]
    public void TestDeliverExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Deliver(null!));
    }
}