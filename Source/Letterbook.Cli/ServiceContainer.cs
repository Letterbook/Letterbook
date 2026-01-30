using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Api;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Workers;
using Letterbook.Workers.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Letterbook.Cli;

public static class ServiceContainer
{
	public static async Task<IHost> CreateAsync()
	{
		var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
		{
			EnvironmentName = Environments.Development,
		});

		/*
		 * [!] It is expected that you run this from Letterbook repository root directory, not "Source/Letterbook.Cli".

		   @todo: Make it so the file is found based on the executable path instead.
		*/
		builder.Configuration.AddJsonFile(Path.Combine("Source", "Letterbook.Cli", "appsettings.Development.json"), optional: false);

		/*
			Also used here: Source\Letterbook.Api\DependencyInjectionExtensions.cs

			@todo: perhaps doesn't make sense for this project to depend on Letterbook.Api.
			That's where `AddServices` comes from.
		*/
		builder.Services.AddServices(builder.Configuration);

		// AccountEventPublisher': Unable to resolve service for type 'MassTransit.IBus' while attempting to activate 'Letterbook.Workers.Publishers.AccountEventPublisher'.
		builder.Services.AddMassTransit(bus =>
		{
			bus.AddWorkerBus(builder.Configuration);
		}).AddPublishers();

		// System.InvalidOperationException: Unable to resolve service for type 'Letterbook.Core.Adapters.IAccountEventPublisher' while attempting to activate 'Letterbook.Core.AccountService'.
		builder.Services.AddScoped<IAccountEventPublisher, AccountEventPublisher>();

		builder.Services.ConfigureAccountManagement(builder.Configuration);

		// Unable to resolve service for type 'Microsoft.AspNetCore.Identity.UserManager`1[Letterbook.Core.Models.Account]' while attempting to activate 'Letterbook.Core.AccountService'
		builder.Services.AddIdentity<Account, IdentityRole<Guid>>(options => { options.User.RequireUniqueEmail = false; })
			.AddEntityFrameworkStores<RelationalContext>()
			.AddDefaultTokenProviders();

		//  Unable to resolve service for type 'Letterbook.Core.Adapters.IActivityPubClient'
		builder.Services.AddHttpClient<IActivityPubClient, Client>(client =>
			{
				// @todo: May not need `IActivityPubClient`.

				// @todo: uncomment next three
				// client.DefaultRequestHeaders.Accept.ParseAdd(Constants.ActivityPubAccept);
				// // TODO: get version from Product Version
				// client.DefaultRequestHeaders.UserAgent.TryParseAdd($"Letterbook/0.0-dev ({coreOptions.DomainName})");
			})
			.AddSigningClient();

		var host = builder.Build();

		await host.StartAsync();

		return host;
	}
}
