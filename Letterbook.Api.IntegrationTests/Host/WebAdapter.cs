using Letterbook.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Letterbook.Api.IntegrationTests.Host;

public class WebAdapter : IDisposable
{
    public HttpClient Client { get; }

    public Mock<AccountService> _mockAccountService;

    public WebAdapter()
    {
        _mockAccountService = new Mock<AccountService>();
        var web = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(s =>
            {
                s.RemoveAll<IAccountService>();
                s.AddScoped<IAccountService>(_ => _mockAccountService.Object);
            }));

        Client = web.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}