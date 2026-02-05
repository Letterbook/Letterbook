using Letterbook.AspNet.Tests.Fixtures;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.IntegrationTests.Fixtures;

/// <summary>
/// A fixture that allows services to be replaced before startup. It is intended to isolate the network adapter part from the application core.
///
/// Differs from HostFixture in that all services are faked. HostFixture is more of an end-to-end, in-memory hosted style.
///
/// Apart from that it is not possible currently to replace services post construction because the constructor calls CreateScope.
///
/// CreateScope starts the application and therefore calls HostFixture.ConfigureWebHost.
///
/// ConfigureWebHost is called at most once so we lose the opportunity to configure services after construction.
/// </summary>
/// <param name="init"></param>
public class ApiFixture : WebApplicationFactory<Program>
{
	public Mock<IAccountService> MockAccountService { get; } = new();
	public Mock<IAuthorizationService> MockAuthorizationService { get; } = new(MockBehavior.Strict);
	public Mock<IAuthzPostService> MockIAuthzPostService { get; } = new(MockBehavior.Strict);
	public Mock<IPostService> MockPostService { get; } = new(MockBehavior.Strict);

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
			UnplugAdapterServices();
			FakeDomainServices();
			FakeAuthentication(s);

			foreach (var action in _initializers)
			{
				action(s);
			}
		});
	}

	// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0
	private void FakeAuthentication(IServiceCollection services)
	{
		services.AddAuthentication()
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

		// See ProfileIdentityMiddleware
		MockAccountService.Setup(it => it.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(new Models.Account());

		// [!] Without this, requests are redirected to log-in screen for some reason.
		services.ConfigureApplicationCookie(options =>
		{
			options.ForwardAuthenticate = "Test";
		});
	}

	protected override void ConfigureClient(HttpClient client)
	{
		base.ConfigureClient(client);

		client.DefaultRequestHeaders.Authorization = new(
			"Test",
			// @todo: show that this MUST be a guid. See ProfileIdentityMiddleware for where it fails.
			"86b77ec9-20d8-45b7-bd5d-d33cf8f6336c");
	}

	private void FakeDomainServices()
	{
		ReplaceScoped(new Mock<IAccountEventPublisher>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IActivityPubAuthenticatedClient>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IActivityPubClient>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IActivityPubDocument>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IActivityScheduler>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IApCrawlScheduler>(MockBehavior.Strict).Object);
		ReplaceScoped(MockIAuthzPostService.Object);
		ReplaceScoped(MockAuthorizationService.Object);
		ReplaceScoped(new Mock<IFeedsAdapter>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IGlobalSearchProvider>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IHostSigningKeyProvider>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IModerationEventPublisher>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IPostEventPublisher>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IPostSearchProvider>(MockBehavior.Strict).Object);
		ReplaceScoped(MockPostService.Object);
		ReplaceScoped(new Mock<IProfileEventPublisher>(MockBehavior.Strict).Object);
		ReplaceScoped(new Mock<IProfileSearchProvider>(MockBehavior.Strict).Object);
	}

	/// <summary>
	/// Adapters services are dependencies OF dependencies and should never be invoked because the domain service layer has been replaced.
	///
	/// For example a controller would never invoke IDataAdapter directly.
	///
	/// These are configured to fail if used at all.
	/// </summary>
	private void UnplugAdapterServices()
	{
		ReplaceScoped(new Mock<IDataAdapter>(MockBehavior.Strict).Object);

		/*

			[!] Required otherwise Letterbook.IntegrationTests.InboxTests.ShouldAcceptReports fails.

			Copied from HostFixture.

		*/
		var seedDescriptor = s.SingleOrDefault(d => d.ImplementationType == typeof(WorkerScope<SeedAdminWorker>));
		if (seedDescriptor != null) s.Remove(seedDescriptor);
	}

	private void ReplaceScoped<TServiceType>(TServiceType replacement) where TServiceType : class
	{
		_initializers.Add(s =>
		{
			s.RemoveAll<TServiceType>();
			s.AddScoped<TServiceType>(_ => replacement);
		});
	}
}