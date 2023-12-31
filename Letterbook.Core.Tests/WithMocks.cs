﻿using ActivityPub.Types;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Workers;
using Microsoft.Extensions.DependencyInjection;
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
    protected ServiceCollection MockedServiceCollection;

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

        MockedServiceCollection = new ServiceCollection();
        MockedServiceCollection.AddScoped<IAccountProfileAdapter>(_ => AccountProfileMock.Object);
        MockedServiceCollection.AddScoped<IMessageBusAdapter>(_ => MessageBusAdapterMock.Object);
        MockedServiceCollection.AddScoped<IAccountEventService>(_ => AccountEventServiceMock.Object);
        MockedServiceCollection.AddScoped<IActivityPubClient>(_ => ActivityPubClientMock.Object);
        MockedServiceCollection.AddScoped<IActivityPubAuthenticatedClient>(_ => ActivityPubAuthClientMock.Object);
        MockedServiceCollection.AddScoped<IProfileService>(_ => ProfileServiceMock.Object);
        MockedServiceCollection.AddScoped<IActivityService>(_ => ActivityServiceMock.Object);
        MockedServiceCollection.TryAddTypesModule();
    }
}