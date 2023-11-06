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
    protected Mock<IActivityPubAuthenticatedClient> ActivityPubAuthClientMock;
    protected Mock<IProfileService> ProfileServiceMock;
    protected Mock<IActivityService> ActivityServiceMock;
    protected IOptions<CoreOptions> CoreOptionsMock;
    protected Mock<HttpMessageHandler> HttpMessageHandlerMock;

    protected WithMocks()
    {
        HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        AccountProfileMock = new Mock<IAccountProfileAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
        AccountEventServiceMock = new Mock<IAccountEventService>();
        ActivityPubClientMock = new Mock<IActivityPubClient>();
        ActivityPubAuthClientMock = new Mock<IActivityPubAuthenticatedClient>();
        ProfileServiceMock = new Mock<IProfileService>();
        ActivityServiceMock = new Mock<IActivityService>();
        
        ActivityPubClientMock.Setup(m => m.As(It.IsAny<Profile>())).Returns(ActivityPubAuthClientMock.Object);
        var mockOptions = new CoreOptions
        {
            DomainName = "letterbook.example",
            Port = "80",
            Scheme = "http"
        };
        CoreOptionsMock = Options.Create(mockOptions);
    }
}