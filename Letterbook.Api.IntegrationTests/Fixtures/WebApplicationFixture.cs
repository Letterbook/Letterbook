using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Api.IntegrationTests.Fixtures;

public class WebApplicationFixture : WebApplicationFactory<Program>, IIntegrationTestData
{
	public List<Profile> Profiles { get; set; } = new();
	public List<Account> Accounts { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();

	public WebApplicationFixture(IMessageSink sink)
	{
		sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
		this.InitTestData();
	}
}