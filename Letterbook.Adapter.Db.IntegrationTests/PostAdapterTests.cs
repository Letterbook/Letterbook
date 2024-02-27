using Letterbook.Adapter.Db.IntegrationTests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.Db.IntegrationTests;

public class PostAdapterTests : IClassFixture<PostgresFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PostgresFixture _postgres;
    private PostAdapter _adapter;
    private RelationalContext _context;
    private RelationalContext _actual;

    public PostAdapterTests(ITestOutputHelper output, PostgresFixture postgres)
    {
        _output = output;
        _postgres = postgres;

        _context = postgres.CreateContext();
        _actual = postgres.CreateContext();
        _adapter = new PostAdapter(Mock.Of<ILogger<PostAdapter>>(), _context);
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_adapter);
    }

    [Fact(DisplayName = "Should lookup posts by ID")]
    public async Task CanLookupById()
    {
        
    }
}