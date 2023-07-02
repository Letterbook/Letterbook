using Bogus;
using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Letterbook.Api.Tests.Fakes;
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
        _activityService = new ActivityService(FediAdapter.Object, ActivityAdapter.Object, ShareAdapter.Object, _logger.Object);

        Randomizer.Seed = new Random(_randomSeed);
        _outputHelper.WriteLine($"Bogus random seed: {_randomSeed}");

    }

    public static IEnumerable<object[]> ActivityList(int count)
    {
        var activityGenerator = new FakeActivity();
        return activityGenerator.GenerateForever().Take(count).Select(a => new[] { a });
    }

    [Fact(DisplayName = "Create method should exist")]
    public void TestCreateExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Create());
    }
    
    [Theory(DisplayName = "Receive can handle common and well formed activities")]
    [MemberData(nameof(ActivityList), 10)]
    public async Task TestReceiveCanHandleActivities(Activity activity)
    {
        var exceptions = await Record.ExceptionAsync(() => _activityService.Receive(activity));
        Assert.Null(exceptions);
    }
    
    [Fact(DisplayName = "Deliver method should exist")]
    public void TestDeliverExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Deliver(null!));
    }
}