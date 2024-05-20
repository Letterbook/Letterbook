using System.Text.Json.Serialization;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public class AccountEventPublisherTests : WithMocks
{
	private readonly ServiceProvider _provider;
	private readonly IAccountEventPublisher _publisher;
	private readonly ITestHarness _harness;
	private readonly FakeAccount _account;

	public AccountEventPublisherTests(ITestOutputHelper output)
	{
		_provider = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<IAccountEventPublisher, AccountEventPublisher>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.UsingInMemory((_, configurator) =>
				{
					configurator.ConfigureJsonSerializerOptions(options =>
					{
						options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

						return options;
					});
				});
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IAccountEventPublisher>();
		_harness = _provider.GetRequiredService<ITestHarness>();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_account = new FakeAccount();

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
	}

	[Fact(DisplayName = "Should publish Created events")]
	public async Task CanPublishCreated()
	{
		await _harness.Start();
		await _publisher.Created(_account);

		Assert.True(await _harness.Published.Any<AccountEvent>(msg => msg.Context.Message.Type == "Created"));
	}

	[Fact(DisplayName = "Should publish Deleted events")]
	public async Task CanPublishDeleted()
	{
		await _harness.Start();
		await _publisher.Deleted(_account, []);

		Assert.True(await _harness.Published.Any<AccountEvent>(msg => msg.Context.Message.Type == "Deleted"));
	}

	[Fact(DisplayName = "Should publish Suspended events")]
	public async Task CanPublishSuspended()
	{
		await _harness.Start();
		await _publisher.Suspended(_account, []);

		Assert.True(await _harness.Published.Any<AccountEvent>(msg => msg.Context.Message.Type == "Suspended"));
	}

	[Fact(DisplayName = "Should publish Updated events")]
	public async Task CanPublishUpdated()
	{
		await _harness.Start();
		await _publisher.Updated(_account, _account, []);

		Assert.True(await _harness.Published.Any<AccountEvent>(msg => msg.Context.Message.Type == "Updated"));
	}

	[Fact(DisplayName = "Should publish Verified events")]
	public async Task CanPublishVerified()
	{
		await _harness.Start();
		await _publisher.Verified(_account, []);

		Assert.True(await _harness.Published.Any<AccountEvent>(msg => msg.Context.Message.Type == "Verified"));
	}
}