namespace Letterbook.Core.Tests;

public class ActivityTest
{
    private Activity _activity;

    public ActivityTest()
    {
        _activity = new Activity();
    }
    [Fact(DisplayName = "Create method should exist")]
    public void TestCreateExists()
    {
        Assert.Throws<NotImplementedException>(() => { _activity.Create(); });
    }
    
    [Fact(DisplayName = "Receive method should exist")]
    public void TestReceiveExists()
    {
        Assert.Throws<NotImplementedException>(() => { _activity.Receive(null!); });
    }
    
    [Fact(DisplayName = "Deliver method should exist")]
    public void TestDeliverExists()
    {
        Assert.Throws<NotImplementedException>(() => { _activity.Deliver(null!); });
    }
}