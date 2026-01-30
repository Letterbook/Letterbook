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
	/*

		This is mimicking what happens in Letterbook.Api.DependencyInjectionExtensions.ConfigureHostBuilder which invoked from
		Letterbook.Api.Program. (See Source/Letterbook.Api/DependencyInjectionExtensions.cs.)

		It tries to create the bare minimum relevant services required to perform domain use cases.

		Ideally it should not include things that are not relevant like anything to do with AspNetCore.

	*/
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
			Also used here: Source/Letterbook.Api/DependencyInjectionExtensions.cs

			@todo: AddServices is in namespace Letterbook.Api. It perhaps doesn't make sense for this project to depend on Letterbook.Api.
		*/
		builder.Services.AddServices(builder.Configuration);

		builder.AddOtherRequiredServices();

		var host = builder.Build();

		await host.StartAsync();

		return host;
	}

	/// <summary>
	/// Additional dependencies that required otherwise they cause failures.
	/// </summary>
	/// <param name="builder"></param>
	private static void AddOtherRequiredServices(this HostApplicationBuilder builder)
	{
		// AccountEventPublisher': Unable to resolve service for type 'MassTransit.IBus' while attempting to activate 'Letterbook.Workers.Publishers.AccountEventPublisher'.
		builder.Services.AddMassTransit(bus =>
		{
			bus.AddWorkerBus(builder.Configuration);
		}).AddPublishers();

		// Unable to resolve service for type 'Microsoft.AspNetCore.Identity.UserManager`1[Letterbook.Core.Models.Account]' while attempting to activate 'Letterbook.Core.AccountService'
		builder.Services.AddIdentity<Account, IdentityRole<Guid>>(options => { options.User.RequireUniqueEmail = false; })
			.AddEntityFrameworkStores<RelationalContext>()
			.AddDefaultTokenProviders();

		//  Unable to resolve service for type 'Letterbook.Core.Adapters.IActivityPubClient'
		builder.Services.AddHttpClient<IActivityPubClient, Client>();
	}
}
