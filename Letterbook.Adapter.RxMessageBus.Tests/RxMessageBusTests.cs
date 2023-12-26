using Microsoft.Extensions.Logging;
using Moq;

namespace Letterbook.Adapter.RxMessageBus.Tests;

public class RxMessageBusTests
{
    private RxMessageBus _bus;

    public RxMessageBusTests()
    {
        _bus = new RxMessageBus(Mock.Of<ILogger<RxMessageBus>>(), new RxMessageChannels(Mock.Of<ILogger<RxMessageChannels>>()));
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_bus);
    }

    [Fact(Skip = "Todo")]
    public void CanListen()
    {
        Assert.Fail("todo");
    }
}