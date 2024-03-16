using ActivityPub.Types;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IActivityAdapter> ActivityAdapterMock;
    protected Mock<IPostAdapter> PostAdapterMock;
    protected Mock<IAccountProfileAdapter> AccountProfileMock;
    protected Mock<IMessageBusAdapter> MessageBusAdapterMock;
    protected Mock<IAccountEventService> AccountEventServiceMock;
    protected Mock<IActivityPubClient> ActivityPubClientMock;
    protected Mock<IActivityPubAuthenticatedClient> ActivityPubAuthClientMock;
    protected Mock<IProfileService> ProfileServiceMock;
    protected IOptions<CoreOptions> CoreOptionsMock;
    protected Mock<HttpMessageHandler> HttpMessageHandlerMock;
    protected ServiceCollection MockedServiceCollection;
    protected Mock<IPostEventService> PostEventServiceMock;
    protected Mock<IPostService> PostServiceMock;
    protected Mock<IAuthorizationService> AuthorizationServiceMock;

    protected WithMocks()
    {
        HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        PostAdapterMock = new Mock<IPostAdapter>();
        AccountProfileMock = new Mock<IAccountProfileAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
        AccountEventServiceMock = new Mock<IAccountEventService>();
        ActivityPubClientMock = new Mock<IActivityPubClient>();
        ActivityPubAuthClientMock = new Mock<IActivityPubAuthenticatedClient>();
        ProfileServiceMock = new Mock<IProfileService>();
        PostEventServiceMock = new Mock<IPostEventService>();
        PostServiceMock = new Mock<IPostService>();
        AuthorizationServiceMock = new Mock<IAuthorizationService>();

        ActivityPubClientMock.Setup(m => m.As(It.IsAny<Profile>())).Returns(ActivityPubAuthClientMock.Object);
        var mockOptions = new CoreOptions
        {
            DomainName = "letterbook.example",
            Port = "80",
            Scheme = "http"
        };
        CoreOptionsMock = Options.Create(mockOptions);

        MockedServiceCollection = new ServiceCollection();
        MockedServiceCollection.AddScoped<IAccountProfileAdapter>(_ => AccountProfileMock.Object);
        MockedServiceCollection.AddScoped<IMessageBusAdapter>(_ => MessageBusAdapterMock.Object);
        MockedServiceCollection.AddScoped<IAccountEventService>(_ => AccountEventServiceMock.Object);
        MockedServiceCollection.AddScoped<IActivityPubClient>(_ => ActivityPubClientMock.Object);
        MockedServiceCollection.AddScoped<IActivityPubAuthenticatedClient>(_ => ActivityPubAuthClientMock.Object);
        MockedServiceCollection.AddScoped<IProfileService>(_ => ProfileServiceMock.Object);
        MockedServiceCollection.TryAddTypesModule();
    }
}