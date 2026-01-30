// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Letterbook.Cli.Commands;

var root = new RootCommand("Root command for CLI")
{
	Subcommands =
	{
		AccountsCommand.Create()
	}
};

root.Parse(args).Invoke();