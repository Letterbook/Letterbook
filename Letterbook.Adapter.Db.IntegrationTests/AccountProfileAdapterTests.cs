using Letterbook.Adapter.Db.IntegrationTests.Fixtures;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.Db.IntegrationTests;

public class AccountProfileAdapterTests : IClassFixture<PostgresFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PostgresFixture _postgres;
    private AccountProfileAdapter _adapter;
    private RelationalContext _context;
    private RelationalContext _actual;
    private List<Profile> _profiles;
    private List<Account> _accounts;
    
    public AccountProfileAdapterTests(ITestOutputHelper output, PostgresFixture postgres)
    {
        _output = output;
        _postgres = postgres;

        _profiles = postgres.Profiles;
        _accounts = postgres.Accounts;
        _context = _postgres.CreateContext();
        _actual = _postgres.CreateContext();
        _adapter = new AccountProfileAdapter(Mock.Of<ILogger<AccountProfileAdapter>>(), _context);
    }
    
    [Fact]
    public void Exists()
    {}

    [Trait("AccountProfileAdapter", "AnyProfile")]
    [Fact(DisplayName = "Should indicate ID is in use")]
    public async Task AnyProfileTest()
    {
        var actual = await _adapter.AnyProfile(_profiles[0].Id);
        
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
        var actual = await _adapter.LookupProfile(_profiles[0].Id);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
    }
    
    [Trait("AccountProfileAdapter", "LookupProfile")]
    [Fact(DisplayName = "Should find a profile by LocalId")]
    public async Task LookupProfileTestLocalId()
    {
        var actual = await _adapter.LookupProfile(_profiles[0].LocalId);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
    }
    
    [Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
    [Fact(DisplayName = "Should find related profiles by LocalId")]
    public async Task LookupProfileForFollowingTestLocalId()
    {
        var actual = await _adapter.LookupProfileWithRelation(_profiles[0].LocalId, _profiles[4].Id);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
        Assert.Contains(_profiles[4], actual.FollowingCollection.Select(r => r.Follows));
        Assert.Contains(_profiles[4], actual.FollowersCollection.Select(r => r.Follower));
    }
    
    [Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
    [Fact(DisplayName = "Should find related profiles by Id")]
    public async Task LookupProfileForFollowingTestId()
    {
        var actual = await _adapter.LookupProfileWithRelation(_profiles[0].Id, _profiles[4].Id);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
        Assert.Contains(_profiles[4], actual.FollowingCollection.Select(r => r.Follows));
        Assert.Contains(_profiles[4], actual.FollowersCollection.Select(r => r.Follower));
    }
    
    [Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
    [Fact(DisplayName = "LookupProfileWithRelation should not permit additional lazy loading")]
    public async Task LookupProfileForFollowingNoLazyLoad()
    {
        var actual = await _adapter.LookupProfileWithRelation(_profiles[0].LocalId, _profiles[4].Id);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
        Assert.Single(actual.FollowingCollection.AsEnumerable());
    }
    
    [Trait("AccountProfileAdapter", "LookupProfileWithRelation")]
    [Fact(DisplayName = "LookupProfileWithRelation by Id should not permit additional lazy loading")]
    public async Task LookupProfileForFollowingNoLazyLoadById()
    {
        var actual = await _adapter.LookupProfileWithRelation(_profiles[0].Id, _profiles[4].Id);
        
        Assert.NotNull(actual);
        Assert.Equal(_profiles[0], actual);
        Assert.Single(actual.FollowingCollection.AsEnumerable());
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
}