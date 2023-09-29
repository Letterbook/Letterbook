using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IActivityAdapter> ActivityAdapterMock;
    protected Mock<IAccountProfileAdapter> AccountProfileMock;
    protected Mock<IMessageBusAdapter> MessageBusAdapterMock;
    protected Mock<IAccountEventService> AccountEventServiceMock;
    protected Mock<IActivityPubClient> ActivityPubClientMock;
    protected Mock<IProfileService> ProfileServiceMock;
    protected Mock<IActivityService> ActivityServiceMock;
    protected IOptions<CoreOptions> CoreOptionsMock;

    protected WithMocks()
    {
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        AccountProfileMock = new Mock<IAccountProfileAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
        AccountEventServiceMock = new Mock<IAccountEventService>();
        ActivityPubClientMock = new Mock<IActivityPubClient>();
        ProfileServiceMock = new Mock<IProfileService>();
        ActivityPubClientMock.Setup(m => m.As(It.IsAny<Profile>())).Returns(ActivityPubClientMock.Object);
        ActivityServiceMock = new Mock<IActivityService>();
        var mockOptions = new CoreOptions
        {
            DomainName = "letterbook.example",
            Port = "80",
            Scheme = "http"
        };
        CoreOptionsMock = Options.Create(mockOptions);
    }
}