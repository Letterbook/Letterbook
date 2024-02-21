using Letterbook.Adapter.Db;
using Letterbook.Api;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Adapter.ActivityPub.IntegrationTests;

public class HostFixture : WebApplicationFactory<Program>
{
    private static readonly Lazy<HostMocks> Lazy = new Lazy<HostMocks>();

    public static HostMocks Mocks => Lazy.Value;

    public HostFixture(IMessageSink sink)
    {
        sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
    }

    /// <summary>
    /// Mock the db adapter, leave everything else with the real implementation
    /// This should make it a lot easier to manage test data
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.ConfigureTestServices(services =>
        {
            if (services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(IAccountProfileAdapter)) is { } adapter)
            {
                services.Remove(adapter);
            }
            
            if (services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(IActivityAdapter)) is { } act)
            {
                services.Remove(act);
            }
            
            if (services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(SeedAdminWorker)) is { } seed)
            {
                services.Remove(seed);
            }

            services.AddScoped<IAccountProfileAdapter>(_ => Mocks.AccountProfileMock.Object);
            services.AddScoped<IActivityAdapter>(_ => Mocks.ActivityAdapterMock.Object);
            services.AddScoped<SeedAdminWorker>(p => 
                new SeedAdminWorker(p.GetService<ILogger<SeedAdminWorker>>(), p.GetService<IOptions<CoreOptions>>(), Mock.Of<IAccountService>(), p.GetService<IAccountProfileAdapter>())
            );
        });
        base.ConfigureWebHost(builder);
    }
}

public class HostMocks : WithMocks
{
    public new Mock<HttpMessageHandler> HttpMessageHandlerMock => base.HttpMessageHandlerMock;

    public new IOptions<CoreOptions> CoreOptionsMock => base.CoreOptionsMock;

    public new Mock<IProfileService> ProfileServiceMock => base.ProfileServiceMock;

    public new Mock<IActivityPubAuthenticatedClient> ActivityPubAuthClientMock => base.ActivityPubAuthClientMock;

    public new Mock<IActivityPubClient> ActivityPubClientMock => base.ActivityPubClientMock;

    public new Mock<IAccountEventService> AccountEventServiceMock => base.AccountEventServiceMock;

    public new Mock<IMessageBusAdapter> MessageBusAdapterMock => base.MessageBusAdapterMock;

    public new Mock<IAccountProfileAdapter> AccountProfileMock => base.AccountProfileMock;

    public new Mock<IActivityAdapter> ActivityAdapterMock => base.ActivityAdapterMock;
}