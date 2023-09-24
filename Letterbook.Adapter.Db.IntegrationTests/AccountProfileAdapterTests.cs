using Letterbook.Adapter.Db.IntegrationTests.Fixtures;
using Letterbook.Core.Tests.Fakes;
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
    
    public AccountProfileAdapterTests(ITestOutputHelper output, PostgresFixture postgres)
    {
        _output = output;
        _postgres = postgres;

        _context = _postgres.CreateContext();
        _actual = _postgres.CreateContext();
        _adapter = new AccountProfileAdapter(Mock.Of<ILogger<AccountProfileAdapter>>(), _context);
    }
    
    [Fact]
    public void Exists()
    {}

    [Fact(DisplayName = "")]
    public async Task AnyProfileTest()
    {
        return;
    }
}