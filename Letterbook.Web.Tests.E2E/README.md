# E2E Testing for Letterbook.Web

## Setup

This test package uses [Playwright](https://playwright.dev). Browsers are automaticaly installed during the test run, but you can also install them manually.

You can find out more about this at https://playwright.dev/dotnet/docs/intro.

Or use

```shell
dotnet build
pwsh bin/Debug/netX/playwright.ps1 install
```

To install browser dependencies. This requires powershell to be installed.

> Note: You need to replace `netX` above with the version of .NET you are using. For example `pwsh .\bin\Debug\net7.0\playwright.ps1 install`

## Examples

### Choose browser and run headed

```shell
BROWSER=firefox HEADED=1 dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj
```

[More options](https://playwright.dev/dotnet/docs/test-runners#customizing-browserlaunch-options-1).

### Filter by name

```shell
BROWSER=firefox HEADED=1 dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj --filter Name~Landing
```

[More options](https://playwright.dev/dotnet/docs/running-tests).

## Common Errors

### Executable doesn't exist at **/chrome.exe | **/firefox.exe

Playwright uses custom headless versions of common browsers to execute its tests. If your seeing an error that chrome/firefox etc can not be found this does not mean you need to reinstall your browsers. Instead you'll need to run the configuration script provided by Playwright to install the necessary browsers for the test clients to run.

Playwright should have provided a hint to install the needed browser tooling or you can use the Setup steps above.

### The argument 'bin/Debug/netX/playwright.ps1' is not recognized as the name of a script file.

`netX` is a placeholder for the version of .NET you are using. You should replace that with the correct version and try again.

### Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:5432

When running on the build server this is because it expecting a database to exist first.
