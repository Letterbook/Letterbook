// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Letterbook.Cli.Commands;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using var host = await Letterbook.Cli.ServiceContainer.CreateAsync();

var provider = host.Services.CreateScope().ServiceProvider;

var root = new RootCommand("Root command for CLI")
{
	Subcommands =
	{
		AccountsCommand.Create(
			provider.GetService<IAccountService>()!,
			provider.GetService<IDataAdapter>()!,
			provider.GetService<ILogger<AccountsCommand.CliLogCategory>>()!),
	}
};

await root.Parse(args).InvokeAsync();
