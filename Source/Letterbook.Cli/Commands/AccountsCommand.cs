using System.CommandLine;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Letterbook.Cli.Commands;

public static class AccountsCommand
{
	public static Command Create(IAccountService accountService, IDataAdapter dataAdapter)
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

		Command createCommand = new("create")
		{
			Description = "Create a new account",
			Arguments = { usernameOption },
			Options = { emailOption }
		};

		createCommand.SetAction(async result =>
		{
			var name = result.GetValue(usernameOption);
			var email = result.GetValue(emailOption);

			Console.WriteLine($"Creating account with username <{name}> and email <{email}>");

			var newAccount = await CreateNewAccountAsync(accountService, dataAdapter, email, name);

			if (newAccount.Succeeded)
			{
				Console.WriteLine($"Created account with id <{newAccount.Succeeded}>");
			}
			else
			{
				Console.WriteLine($"Failed: <{string.Join(", ", newAccount.Errors.Select(it => it.Description))}>");
			}
		});

		return new("accounts")
		{
			Description = "Account commands",
			Subcommands = { createCommand }
		};
	}

	private static async Task<IdentityResult> CreateNewAccountAsync(
		IAccountService accountService,
		IDataAdapter dataAdapter,
		string? email, string? name)
	{
		dataAdapter.Add(new InviteCode("dummy-for-cli"));

		await dataAdapter.Commit();

		var inviteCode = dataAdapter.InviteCodes("dummy-for-cli").SingleOrDefault();

		Console.WriteLine($"Added dummy invite code <{inviteCode}>");

		return await accountService.RegisterAccount(email, name, "xxx-todo-xxx", "dummy-for-cli");
	}
}