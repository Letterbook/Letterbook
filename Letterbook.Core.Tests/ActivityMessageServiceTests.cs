using ActivityPub.Types.Conversion;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;

namespace Letterbook.Core.Tests;

public class ActivityMessageServiceTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
    private readonly IJsonLdSerializer _serializer;
    private ActivityMessageService _service;

    public ActivityMessageServiceTests(JsonLdSerializerFixture serializer)
    {
        _serializer = serializer.JsonLdSerializer;
        _service = new ActivityMessageService(Mock.Of<ILogger<ActivityMessageService>>(), CoreOptionsMock, _serializer, Mock.Of<IMessageBusAdapter>());
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_service);
    }

    [Fact]
    public void CanDeliver()
    {
        Assert.Fail("todo");
    }
}