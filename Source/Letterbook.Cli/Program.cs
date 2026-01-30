// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Letterbook.Cli.Commands;
using Letterbook.Core;
using Microsoft.Extensions.DependencyInjection;

using var host = await Letterbook.Cli.ServiceContainer.CreateAsync();

// System.InvalidOperationException: Cannot resolve scoped service 'Letterbook.Core.IAccountService' from root provider.
var provider = host.Services.CreateScope().ServiceProvider;

var root = new RootCommand("Root command for CLI")
{
	Subcommands =
	{
		AccountsCommand.Create(provider.GetService<IAccountService>())
	}
};

await root.Parse(args).InvokeAsync();
