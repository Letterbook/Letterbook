using System.CommandLine;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Letterbook.Cli.Commands;

public static class AccountsCommand
{
	public class CliLogCategory
	{

	}

	public static Command Create(IAccountService accountService, IDataAdapter dataAdapter, ILogger<CliLogCategory> log)
	{
		return new("accounts")
		{
			Description = "Account commands",
			Subcommands =
			{
				CreateCommand(log, accountService, dataAdapter),
				ListCommand(log, dataAdapter)
			}
		};
	}

	private static Command CreateCommand(ILogger log, IAccountService accountService, IDataAdapter dataAdapter)
	{
		var usernameOption = new Argument<string>("NAME")
		{
			Description = "The person's name"
		};

		var emailOption = new Option<string>("--email")
		{
			Required = true,
			Description = "Email address of the new account"
		};

		Command createCommand = new Command("create")
		{
			Description = "Create a new account",
			Arguments = { usernameOption },
			Options = { emailOption },
		};

		createCommand.SetAction(async result =>
		{
			var name = result.GetRequiredValue(usernameOption);
			var email = result.GetRequiredValue(emailOption);
			var password = "$1Password";

			Console.WriteLine($"Creating account with username <{name}> and email <{email}>");

			var newAccount = await CreateNewAccountAsync(accountService, dataAdapter, email, name, password);

			Console.WriteLine(newAccount.Succeeded
				? $"Created account with handle <{name}>, email <{email}> and password <{password}>"
				: $"Failed: <{string.Join(", ", newAccount.Errors.Select(it => it.Description))}>");
		});

		return createCommand;
	}

	private static Command ListCommand(ILogger log, IDataAdapter dataAdapter)
	{
		Command listCommand = new("list")
		{
			Description = "List accounts"
		};

		var allAccounts = dataAdapter.AllAccounts();

		log.LogDebug($"Found <{allAccounts.Count()}> accounts");

		listCommand.SetAction(_ =>
		{
			foreach (var account in allAccounts)
			{
				Console.WriteLine($"{account.Id} {account.Name} {account.Email}");
			}
		});

		return listCommand;
	}

	private static async Task<IdentityResult> CreateNewAccountAsync(
		IAccountService accountService,
		IDataAdapter dataAdapter,
		string email, string name, string password)
	{
		dataAdapter.Add(new InviteCode("dummy-for-cli"));

		await dataAdapter.Commit();

		return await accountService.RegisterAccount(email, name, password, "dummy-for-cli");
	}
}