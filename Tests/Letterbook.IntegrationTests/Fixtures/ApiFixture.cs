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
/// A fixture that allows services to be replaced before the application is started in-memory.
///
/// It is intended to isolate the network adapter (Microsoft.AspNetCore) part from the application core.
///
/// That is so you can write tests like:
///
///		"A GET request to /lb/v1/user_account/register invokes these domain operations"
///
/// Use ReplaceScoped to replace a service dependency as part of a test:
///
///		_fixture.ReplaceScoped(Mock.Of<IPostService>());
///
///		using var _client = _fixture.CreateClient();
///
/// Differs from HostFixture in that all services are faked. HostFixture is more of an end-to-end, in-memory hosted style.
///
/// Apart from that it is not possible currently to replace services post construction because the constructor calls CreateScope.
///
/// CreateScope starts the application and therefore calls HostFixture.ConfigureWebHost.
///
/// ConfigureWebHost is called at most once so we lose the opportunity to configure services after construction.
/// </summary>
public class ApiFixture : WebApplicationFactory<Program>
{
	/// <summary>
	/// Take care replacing this one because it is used in authentication. See ApiFixture.FakeAuthentication.
	/// </summary>
	public Mock<IAccountService> MockAccountService { get; } = new(MockBehavior.Strict);
	public Mock<IAuthorizationService> MockAuthorizationService { get; } = new(MockBehavior.Strict);
	public Mock<IAuthzPostService> MockIAuthzPostService { get; } = new(MockBehavior.Strict);

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
			UnplugAdapterServices(s);
			FakeDomainServices(s);
			FakeAuthentication(s);

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

	// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0
	private void FakeAuthentication(IServiceCollection services)
	{
		services.AddAuthentication()
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

		// User must have an existing account to be authenticated. See ProfileIdentityMiddleware.
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

	/// <summary>
	/// Try and replace all domain services with test doubles. (I have just guessed at this, there may be some I have missed.)
	///
	/// The idea is to completely isolate the Microsoft.AspNetCore part from the core part.
	/// </summary>
	/// <param name="services"></param>
	private void FakeDomainServices(IServiceCollection services)
	{
		services.ReplaceScoped(Mock.Of<IAccountEventPublisher>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IActivityPubAuthenticatedClient>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IActivityPubClient>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IActivityPubDocument>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IActivityScheduler>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IApCrawlScheduler>(MockBehavior.Strict));
		services.ReplaceScoped(MockIAuthzPostService.Object);
		services.ReplaceScoped(MockAuthorizationService.Object);
		services.ReplaceScoped(Mock.Of<IFeedsAdapter>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IGlobalSearchProvider>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IHostSigningKeyProvider>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IModerationEventPublisher>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IPostEventPublisher>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IPostSearchProvider>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IPostService>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IProfileEventPublisher>(MockBehavior.Strict));
		services.ReplaceScoped(Mock.Of<IProfileSearchProvider>(MockBehavior.Strict));
	}

	/// <summary>
	/// Adapters services are dependencies OF dependencies and should never be invoked.
	///
	/// The domain service layer has been replaced with test doubles so there is nothing left to call them.
	///
	/// For example a controller would never invoke IDataAdapter directly.
	///
	/// These are configured to fail if used at all.
	/// </summary>
	private void UnplugAdapterServices(IServiceCollection services)
	{
		services.ReplaceScoped(Mock.Of<IDataAdapter>(MockBehavior.Strict));
	}

	public void ReplaceScoped<TServiceType>(TServiceType replacement) where TServiceType : class
	{
		_initializers.Add(s => s.ReplaceScoped(replacement));
	}
}

internal static class IServiceCollectionExtensions
{
	public static void ReplaceScoped<TServiceType>(this IServiceCollection self, TServiceType replacement) where TServiceType : class
	{
		self.RemoveAll<TServiceType>();
		self.AddScoped<TServiceType>(_ => replacement);
	}
}