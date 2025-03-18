using System.Security.Claims;
using Letterbook.AspNet.Tests.Fixtures;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.AspNet.Tests;

public class ProfileIdentityMiddlewareTests : WithMocks
{
	private IHost _host;
	private TestServer _server;
	private static readonly ClaimComparer ClaimComparer = new();

	public ProfileIdentityMiddlewareTests()
	{
		_host = new HostBuilder()
			.ConfigureWebHost(webBuilder =>
			{
				webBuilder
					.UseTestServer()
					.ConfigureServices(services =>
					{
						services.AddRouting();
						services.AddScoped<IAccountService>(_ => AccountServiceMock.Object);
						services.AddAuthentication("Test")
							.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
					})
					.Configure(app =>
					{
						app.UseRouting();
						app.UseMiddleware<ProfileIdentityMiddleware>();
						app.UseEndpoints(endpoints =>
						{
							endpoints.MapGet("/", () =>
								TypedResults.Text("Test resource"));
						});
					});
			})
			.Start();
		_server = _host.GetTestServer();
	}

	[Fact(DisplayName = "Should ignore unauthenticated requests")]
	public async Task Ignore()
	{
		var context = await _server.SendAsync(c =>
		{
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		var bodyReader = new StreamReader(context.Response.Body);

		Assert.Empty(context.User.Claims);
		Assert.Equal(200, context.Response.StatusCode);
		Assert.Equal("Test resource", await bodyReader.ReadLineAsync());
	}

	[Fact(DisplayName = "Should re-challenge invalid authentication")]
	public async Task Reauthenticate()
	{
		var sub = new Claim("sub", $"{Guid.NewGuid()}");
		AccountServiceMock.Setup(m => m.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(default(Account));

		var context = await _server.SendAsync(c =>
		{
			c.User = new ClaimsPrincipal(new ClaimsIdentity([sub], "Test"));
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		var bodyReader = new StreamReader(context.Response.Body);

		Assert.Equal(401, context.Response.StatusCode);
	}

	[Fact(DisplayName = "Should provide run-time profile claims on authenticated requests")]
	public async Task LookupClaims()
	{
		var account = new FakeAccount().Generate();
		var sub = new Claim("sub", $"{account.Id}");
		AccountServiceMock.Setup(m => m.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(account);

		var context = await _server.SendAsync(c =>
		{
			c.User = new ClaimsPrincipal(new ClaimsIdentity([sub], "Test"));
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		var expected = new Claim("activeProfile", $"{account.LinkedProfiles.First().Profile.GetId25()}");
		Assert.Contains(expected, context.User.Claims, ClaimComparer);
	}

	[Fact(DisplayName = "Should preserve an existing activeProfile claim")]
	public async Task PreserveActiveProfile()
	{
		var account = new FakeAccount(false).Generate();
		var profiles = new FakeProfile("letterbook.example", account).Generate(2);
		foreach (var profile in profiles)
		{
			account.LinkedProfiles.Add(new ProfileClaims(account, profile, [ProfileClaim.Owner]));
		}
		var sub = new Claim("sub", $"{account.Id}");
		var active = new Claim("activeProfile", $"{profiles[1].GetId25()}");
		AccountServiceMock.Setup(m => m.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(account);

		var context = await _server.SendAsync(c =>
		{
			c.User = new ClaimsPrincipal(new ClaimsIdentity([sub, active], "Test"));
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		Assert.Contains(active, context.User.Claims, ClaimComparer);
		Assert.Single(context.User.Claims, claim => ClaimComparer.Equals(claim, active));
	}

	[Fact(DisplayName = "Should preserve an existing guest activeProfile claim")]
	public async Task PreserveGuestActiveProfile()
	{
		var account = new FakeAccount(false).Generate();
		var profiles = new FakeProfile("letterbook.example", account).Generate(2);
		var guest = new FakeProfile("letterbook.example").Generate();
		foreach (var profile in profiles)
		{
			account.LinkedProfiles.Add(new ProfileClaims(account, profile, [ProfileClaim.Owner]));
		}
		account.LinkedProfiles.Add(new ProfileClaims(account, guest, [ProfileClaim.Guest]));
		var sub = new Claim("sub", $"{account.Id}");
		var active = new Claim("activeProfile", $"{guest.GetId25()}");
		AccountServiceMock.Setup(m => m.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(account);

		var context = await _server.SendAsync(c =>
		{
			c.User = new ClaimsPrincipal(new ClaimsIdentity([sub, active], "Test"));
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		Assert.Contains(active, context.User.Claims, ClaimComparer);
		Assert.Single(context.User.Claims, claim => ClaimComparer.Equals(claim, active));
	}

	[Fact(DisplayName = "Should reject an invalid activeProfile claim")]
	public async Task RejectInvalid()
	{
		var account = new FakeAccount(false).Generate();
		var profiles = new FakeProfile("letterbook.example", account).Generate(2);
		var guest = new FakeProfile("letterbook.example").Generate();
		foreach (var profile in profiles)
		{
			account.LinkedProfiles.Add(new ProfileClaims(account, profile, [ProfileClaim.Owner]));
		}
		var sub = new Claim("sub", $"{account.Id}");
		var active = new Claim("activeProfile", $"{guest.GetId25()}");
		AccountServiceMock.Setup(m => m.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(account);

		var context = await _server.SendAsync(c =>
		{
			c.User = new ClaimsPrincipal(new ClaimsIdentity([sub, active], "Test"));
			c.Request.Method = "Get";
			c.Request.Path = "/";
		});

		Assert.Equal(401, context.Response.StatusCode);
	}
}