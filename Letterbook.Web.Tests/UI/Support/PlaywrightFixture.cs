using Microsoft.Playwright;

namespace Letterbook.Web.Tests.UI.Support;

public class PlaywrightFixture : IDisposable, IAsyncLifetime
{
	private class Options
	{
		public bool Headless { get; init; }
	}

	private IPlaywright? _playwright;
	private readonly Options _opts = new() { Headless = Settings.Headless };
	public IBrowser? Browser { get; private set; }

	public async Task InitializeAsync()
	{
		_playwright = await Playwright.CreateAsync();
		Browser = await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _opts.Headless });
	}

	public Task DisposeAsync()
	{
		_playwright?.Dispose();
		return Task.CompletedTask;
	}

	public void Dispose() => _playwright?.Dispose();
}