using Microsoft.Extensions.Logging;
using Moq;

namespace Letterbook.Core.Tests;

public class ActivityTest : WithMocks
{
    private readonly ActivityService _activityService;
    private readonly Mock<ILogger<ActivityService>> _logger;

    public ActivityTest()
    {
        _logger = new Mock<ILogger<ActivityService>>();
        _activityService = new ActivityService(FediAdapter.Object, ActivityAdapter.Object, ShareAdapter.Object, _logger.Object);
    }
    
    [Fact(DisplayName = "Create method should exist")]
    public void TestCreateExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Create());
    }
    
    [Fact(DisplayName = "Receive method should exist")]
    public async Task TestReceiveExists()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() => _activityService.Receive(null!));
    }
    
    [Fact(DisplayName = "Deliver method should exist")]
    public void TestDeliverExists()
    {
        Assert.Throws<NotImplementedException>(() => _activityService.Deliver(null!));
    }
}