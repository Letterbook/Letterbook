# Notes on adding CLI

## Log 

### 30-Jan-2026

Other servers have a cli:

* https://docs.gotosocial.org/en/v0.20.3/admin/cli/
* https://docs.joinmastodon.org/admin/tootctl/#accounts-create

Can we implement one use case, for example [creating an account](https://docs.joinmastodon.org/admin/tootctl/#accounts-create):

```shell
cli accounts create USERNAME --email EMAIL
```

Using .NET because everything else is.

```shell
dotnet run --project Source/Letterbook.Cli/ 
```

Using [this guide to parameters](https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial).

Can we try and reproduce registration?

```csharp
[AllowAnonymous]
[HttpPost]
[ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
public async Task<IActionResult> Register([FromBody] RegistrationRequest registration)
{
    try
    {
        var registerAccount = await _accountService
            .RegisterAccount(registration.Email, registration.Handle, registration.Password, registration.InviteCode);

        if (registerAccount is null) return Forbid();
        if (!registerAccount.Succeeded) return BadRequest(registerAccount.Errors);

        return await Login(new LoginRequest { Email = registration.Email, Password = registration.Password });
    }
    catch (Exception e)
    {
        return BadRequest(e);
    }
}
```

This will mean we need a configuration file for database connection etc.

The core piece is `AccountService`.

Using [the built-in dependency injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview).

Having trouble loading configuration. It seems to be ignored.

Do I really have to do this (`AddJsonFile`)?

```csharp
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
	EnvironmentName = Environments.Development,
});

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
```

Ah, it may be because I am running from the root directory:

```shell
Unhandled exception. System.IO.FileNotFoundException: The configuration file 'appsettings.Development.json' was not found and is not optional. 
The expected physical path was 'C:\Users\BenBiddington\sauce\Letterbook\appsettings.Development.json'.
```

MassTransit.ConfigurationException: The ReceivePipeConfiguration can only be used once.

> Oh, F, I didn't see what you were doing. Calling var serviceProvider = services.BuildServiceProvider(); is a broken pattern, and is not supported. 
> You're basically building the container twice, with the same things. [github.com](https://github.com/MassTransit/MassTransit/discussions/3296)

```csharp
host.StartAsync();

return (host, builder.Services.BuildServiceProvider());
```

Suspect it's the `builder.Services.BuildServiceProvider()` which has already been done in `host.StartAsync()`.

Scoped provider required.

// System.InvalidOperationException: Cannot resolve scoped service 'Letterbook.Core.IAccountService' from root provider.
var provider = host.Services.CreateScope().ServiceProvider;

> Failed: <The invite code is not valid>

### 31-Jan-2026

Can we suppress the "The invite code is not valid" message by adding the necessary data in the background. CLI user doesn't care about invites.

That does mean we need additional dependencies in this case where we're operating with internal services.

Actually it may be required for API as well.

Successfully added invite code, next thing is password:

> Failed: <Passwords must have at least one digit ('0'-'9')., Passwords must have at least one uppercase ('A'-'Z').>

We can either auto generate one or force user to supply.

Added a constant one to bypass that message and now account creation succeeds.

## Processing arguments

## Dependency injection

How are things wired up then?

## Use API?

## Publish as a tool?

## Local only?

Assume that development configuration is all we need at the moment.

## Integration tests

The CLI is purely a client of the domain so we should be able to test it the same way.