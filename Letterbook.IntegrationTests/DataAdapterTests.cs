using Letterbook.Adapter.Db;
using Letterbook.Core.Models;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
	private List<Profile> _profiles;
	private List<Account> _accounts;
	private readonly IServiceScope _scope;
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
	}

	[Fact]
	public void Exists()
	{
	}

	[Trait("AccountProfileAdapter", "AnyProfile")]
	[Fact(DisplayName = "Should indicate ID is in use")]
	public async Task AnyProfileTest()
	{
		var actual = await _adapter.AnyProfile(_profiles[0].FediId);

		Assert.True(actual);
	}

	[Trait("AccountProfileAdapter", "AnyProfile")]
	[Fact(DisplayName = "Should indicate ID is not in use")]
	public async Task AnyProfileTestNone()
	{
		var actual = await _adapter.AnyProfile(new Uri("https://does.notexist.example"));

		Assert.False(actual);
	}

	[Trait("AccountProfileAdapter", "AnyProfile")]
	[Fact(DisplayName = "Should indicate handle is in use")]
	public async Task AnyProfileHandleTest()
	{
		var actual = await _adapter.AnyProfile(_profiles[0].Handle);

		Assert.True(actual);
	}

	[Trait("AccountProfileAdapter", "AnyProfile")]
	[Fact(DisplayName = "Should indicate handle is not in use")]
	public async Task AnyProfileHandleTestNone()
	{
		var actual = await _adapter.AnyProfile("This is not a real handle");

		Assert.False(actual);
	}

	[Trait("AccountProfileAdapter", "LookupProfile")]
	[Fact(DisplayName = "Should find a profile by Id")]
	public async Task LookupProfileTestId()
	{
		var actual = await _adapter.LookupProfile(_profiles[0].FediId);

		Assert.NotNull(actual);
		Assert.Equal(_profiles[0], actual);
	}

	[Trait("AccountProfileAdapter", "LookupProfile")]
	[Fact(DisplayName = "Should find a profile by LocalId")]
	public async Task LookupProfileTestLocalId()
	{
		var actual = await _adapter.LookupProfile(_profiles[0].GetId());

		Assert.NotNull(actual);
		Assert.Equal(_profiles[0], actual);
	}

	[Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
	[Fact(DisplayName = "Should find related profiles by LocalId")]
	public async Task LookupProfileForFollowingTestLocalId()
	{
		var actual = await _adapter.LookupProfileWithRelation(_profiles[0].GetId(), _profiles[4].FediId);

		Assert.NotNull(actual);
		Assert.Equal(_profiles[0], actual);
		Assert.Contains(_profiles[4], actual.FollowingCollection.Select(r => r.Follows));
		Assert.Contains(_profiles[4], actual.FollowersCollection.Select(r => r.Follower));
	}

	[Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
	[Fact(DisplayName = "LookupProfileWithRelation should not permit additional lazy loading")]
	public async Task LookupProfileForFollowingNoLazyLoad()
	{
		var actual = await _adapter.LookupProfileWithRelation(_profiles[0].GetId(), _profiles[4].FediId);

		Assert.NotNull(actual);
		Assert.Equal(_profiles[0], actual);
		Assert.Single(actual.FollowingCollection.AsEnumerable());
		Assert.Single(actual.FollowersCollection.AsEnumerable());
	}

	[Trait("AccountProfileAdapter", "FindProfilesByHandle")]
	[Fact(DisplayName = "Find profile by handle")]
	public async Task FindProfileByHandleTest()
	{
		var actual = _adapter.FindProfilesByHandle(_profiles[0].Handle);
		var list = new List<Profile>();
		await foreach (var each in actual)
		{
			list.Add(each);
		}

		Assert.Contains(_profiles[0], list);
		Assert.DoesNotContain(_profiles[1], list);
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
		var profile = Assert.IsType<Profile>(actual.First());
		Assert.NotEqual(Uuid7.Empty, profile.GetId());
	}
}