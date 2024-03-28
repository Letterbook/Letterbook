using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Api.IntegrationTests.Fixtures;

public class WebApplicationFixture : WebApplicationFactory<Program>, IIntegrationTestData
{
	private readonly IMessageSink _sink;
	private static readonly object _lock = new();
	public List<Profile> Profiles { get; set; } = new();
	public List<Account> Accounts { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();

	public WebApplicationFixture(IMessageSink sink)
	{
		_sink = sink;
	}

	public void InitData(IAccountProfileAdapter profileAdapter, IPostAdapter postAdapter)
	{
		lock(_lock)
		{
			_sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
			this.InitTestData();

		}
	}
}