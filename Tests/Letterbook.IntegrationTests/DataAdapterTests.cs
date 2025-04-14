using AutoMapper;
using AutoMapper.QueryableExtensions;
using Letterbook.Adapter.Db;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Queries;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Adapter")]
public sealed class DataAdapterTests : IClassFixture<HostFixture<DataAdapterTests>>, ITestSeed, IDisposable
{
	public void Dispose()
	{
		_scope.Dispose();
	}

	private readonly ITestOutputHelper _output;
	private readonly HostFixture<DataAdapterTests> _host;
	private DataAdapter _adapter;
	private RelationalContext _context;
	private RelationalContext _actual;
	private List<Models.Profile> _profiles;
	private List<Account> _accounts;
	private readonly IServiceScope _scope;
	private readonly Mapper _mapper;
	private readonly MappingConfigProvider _mappingProvider;
	static int? ITestSeed.Seed() => null;

	public DataAdapterTests(ITestOutputHelper output, HostFixture<DataAdapterTests> host)
	{
		_output = output;
		_host = host;
		_scope = _host.CreateScope();

		_profiles = host.Profiles;
		_accounts = host.Accounts;
		_context = _host.CreateContext(_scope);
		_actual = _host.CreateContext(_scope);
		_adapter = new DataAdapter(Mock.Of<ILogger<DataAdapter>>(), _context);
		_mappingProvider = new MappingConfigProvider(Options.Create(new CoreOptions()));
		_mapper = new Mapper(_mappingProvider.Profiles);
	}

	[Fact]
	public void Exists()
	{
	}

	[Trait("AccountProfileAdapter", "QueryFrom")]
	[Fact(DisplayName = "Query from profiles")]
	public async Task QueryFromProfile()
	{
		// We used to check audience, but that's likely to change as more features and test scenarios are added
		// Headlining is more stable
		var query = _adapter.QueryFrom(_profiles[0], profile => profile.Headlining);
		var actual = await query.ToListAsync();

		Assert.Equal(3, actual.Count);
	}

	[Trait("AccountProfileAdapter", "QueryFrom")]
	[Fact(DisplayName = "Query from profiles Include navigation")]
	public async Task QueryFromProfileAndInclude()
	{
		var query = _adapter.QueryFrom(_profiles[0], profile => profile.FollowersCollection);
		var actual = await query
			.Include(relation => relation.Follower)
			.Select(relation => relation.Follower)
			.ToListAsync();

		Assert.Single(actual);
		var profile = Assert.IsType<Models.Profile>(actual.First());
		Assert.NotEqual(Uuid7.Empty, profile.GetId());
	}

	[Fact(DisplayName = "Query profile projection fields")]
	public async Task QueryProfileProjections()
	{
		var projected = await _adapter.Profiles(_profiles[7].Id).Select(e => new {Followers = e.FollowersCount}).FirstAsync();
		var automapProjected = await _adapter.Profiles(_profiles[7].Id).ProjectTo<FullProfileDto>(_mappingProvider.Profiles).FirstAsync();
		var notProjected = await _adapter.Profiles(_profiles[7].Id).FirstAsync();

		Assert.Equal(4, projected.Followers);
		Assert.Equal(4, automapProjected.Followers);
		Assert.Equal(0, notProjected.FollowersCount);
	}
}