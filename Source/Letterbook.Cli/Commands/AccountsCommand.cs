using System.CommandLine;
using Letterbook.Core;

namespace Letterbook.Cli.Commands;

public static class AccountsCommand
{
	public static Command Create(IAccountService accountService)
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

			var newAccount = await accountService.RegisterAccount(email, name, "", "");

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
}