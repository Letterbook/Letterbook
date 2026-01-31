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

Now you should be able to log in with it.

[Run the server](https://github.com/Letterbook/Letterbook/blob/main/CONTRIBUTING.md#quick-start) and log in there with email and password.

#### Controlling logging

There are lots of logs being written to console, for example

```shell
info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[63]
      User profile is available. Using 'C:\Users\BenBiddington\AppData\Local\ASP.NET\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest.
```

How do you turn those off?

Logging is not done by serilog here, it is the default built-in, so configure like:

```csharp
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
});
```

### 31-Jan-2026 (3.20 PM)

Having a go at making the CLI go via API instead.

We'd like it to be a runtime option, something like 

```shell
dotnet run --project Source/Letterbook.Cli accounts list --useApi
```

--or-- via configuration

```shell
Toggles__UseApi=true dotnet run --project Source/Letterbook.Cli accounts list
```

The latter is easier because the service container will be able to read the configuration to decide.

The former is more difficult because the CLI runtime is after dependencies have been created.

Requires API to be running, not the UI:

```shell
dotnet watch run --project Source/Letterbook.Api --launch-profile dev
```

Snake casing caught me out, the url is:

```
/lb/v1/user_account/register
```

The use of third-party `IdentityResult` is annoying as there is no way to construct one with errors.

The request is succeeding, but I get e=this error as it's trying to return:

```shell
dbug: Letterbook.Cli.Adapters.NetworkAccountService[0]
      System.NotSupportedException: Serialization and deserialization of 'System.Reflection.MethodBase' instances is not supported. Path: $.TargetSite.
       ---> System.NotSupportedException: Serialization and deserialization of 'System.Reflection.MethodBase' instances is not supported.
         at System.Text.Json.Serialization.Converters.UnsupportedTypeConverter`1.Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
         at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
         at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
         at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
         at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
         at System.Text.Json.Serialization.JsonConverter`1.WriteCore(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
         --- End of inner exception stack trace ---
         at System.Text.Json.ThrowHelper.ThrowNotSupportedException(WriteStack& state, Exception innerException)
         at System.Text.Json.Serialization.JsonConverter`1.WriteCore(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
         at Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter.WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeResultAsync>g__Logged|22_0(ResourceInvoker invoker, IActionResult result)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeResultFilters>g__Awaited|28_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
         at Serilog.AspNetCore.RequestLoggingMiddleware.Invoke(HttpContext httpContext)
         at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
         at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
         at Letterbook.Api.Authentication.HttpSignature.Infrastructure.HttpSignatureVerificationMiddleware.InvokeAsync(HttpContext context, RequestDelegate next) in C:\Users\BenBiddington\sauce\Letterbook\Source\Letterbook.Api.Authentication.HttpSignature\Infrastructure\HttpSignatureVerificationMiddleware.cs:line 43
         at Microsoft.AspNetCore.Builder.UseMiddlewareExtensions.InterfaceMiddlewareBinder.<>c__DisplayClass2_0.<<CreateMiddleware>b__0>d.MoveNext()
      --- End of stack trace from previous location ---
         at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
         at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
         at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

      HEADERS
      =======
      Host: localhost:5127
      Content-Type: application/json; charset=utf-8
      Transfer-Encoding: chunked
```

Will try the list method as well.

Can't do this because there is no network endpoint available for listing accounts.

The error is coming from the log on part at `(1)`:

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

			return await Login(new LoginRequest { Email = registration.Email, Password = registration.Password }); // (1)
		}
		catch (Exception e)
		{
			return BadRequest(e);
		}
	}
```

If I comment it out and return ok then everything works.

Will have to look at that next.

## Ideas

### Publish as a tool?

### Integration tests

The CLI is purely a client of the domain so we should be able to test it the same way.

Is it worth verifying that the CLI invokes the correct domain operation?