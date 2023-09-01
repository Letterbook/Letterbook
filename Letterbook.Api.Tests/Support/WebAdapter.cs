using Letterbook.Api.Tests.Fakes;
using Letterbook.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Letterbook.Api.Tests.Support;

public class WebAdapter : IDisposable
{
    public HttpClient Client { get; }

    public FakeAccountService FakeAccountService { get; set; } = new();

    public WebAdapter()
    {
        var web = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(s =>
            {
                s.RemoveAll<IAccountService>();
                s.AddScoped<IAccountService>(_ => FakeAccountService);
            }));

        Client = web.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}