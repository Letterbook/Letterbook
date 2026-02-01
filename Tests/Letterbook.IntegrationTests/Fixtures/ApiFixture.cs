using Letterbook.Core;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Letterbook.IntegrationTests.Fixtures;

/// <summary>
/// A web application factory that allows services to be replaced before startup.
///
/// [!] Modifying HostFixture to do this would require too many changes at the moment.
///
/// The reason it doesn't work is because it calls `CreateScope` in its constructor.
/// This immediately starts the application which means `ConfigureWebHost` is also called as part of constructor.
///
/// There is then no longer an opportunity to replace services because `ConfigureWebHost` is only called once per start-up.
///
/// CustomWebApplicationFactory is different because you can replace services any time until `CreateApiClient` or `CreateClient` is called.
/// </summary>
/// <param name="init"></param>
public class ApiFixture : WebApplicationFactory<Program>
{
	public Mock<IAccountService> MockAccountService { get; } = new();

	private readonly List<Action<IServiceCollection>> _initializers = [];

	public ApiFixture()
	{
		ReplaceScoped(MockAccountService.Object);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);

		builder.ConfigureServices(s =>
		{
			/*

				[!] Required otherwise Letterbook.IntegrationTests.InboxTests.ShouldAcceptReports fails.

				Copied from HostFixture.

			*/
			var seedDescriptor = s.SingleOrDefault(d => d.ImplementationType == typeof(WorkerScope<SeedAdminWorker>));
			if (seedDescriptor != null) s.Remove(seedDescriptor);

			foreach (var action in _initializers)
			{
				action(s);
			}
		});
	}

	public void ReplaceScoped<TServiceType>(TServiceType replacement) where TServiceType : class
	{
		_initializers.Add(s =>
		{
			s.RemoveAll<TServiceType>();
			s.AddScoped<TServiceType>(_ => replacement);
		});
	}
}