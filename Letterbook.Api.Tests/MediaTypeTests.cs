namespace Letterbook.Api.Tests;


public class MediaTypeTests
{
    private AcceptHeaderAttribute _attr = new("application/ld+json",
        "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"", "application/activity+json");

    [Theory]
    [InlineData("application/ld+json")]
    [InlineData("application/ld+json, */*")]
    [InlineData("*/*, application/ld+json")]
    [InlineData("application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"")]
    [InlineData("application/activity+json")]
    [InlineData("application/activity+json, */*")]
    [InlineData("application/activity+json, application/*")]
    public void AttributeAccept(string given)
    {
        Assert.True(_attr.IsMatch(given));
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json, */*")]
    [InlineData("application/ld")]
    [InlineData("application/ld; profile=\"https://www.w3.org/ns/activitystreams\"")]
    [InlineData("application/*")]
    [InlineData("text/json")]
    [InlineData("text/json, */*")]
    [InlineData("*/*")]
    [InlineData("")]
    [InlineData(null)]
    public void AttributeDeny(string given)
    {
        Assert.False(_attr.IsMatch(given));
    }
}