using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class PostServiceTests : WithMocks
{
    private readonly ITestOutputHelper _output;
    private readonly PostService _service;
    private readonly Profile _profile;

    public PostServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _service = new PostService(Mock.Of<ILogger<PostService>>(), CoreOptionsMock, AccountProfileMock.Object,
            PostAdapterMock.Object);
        _profile = new FakeProfile("letterbook.example").Generate();
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_service);
    }

    [Fact(DisplayName = "Should create unpublished draft notes")]
    public async Task CanDraftNote()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.IsAny<Guid>()))
            .ReturnsAsync(_profile);
        
        var actual = await _service.DraftNote(_profile.Id, "Test content");

        var expected = DateTimeOffset.Now;
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Contents);
        Assert.Equal("Test content", actual.Contents.First().Preview);
        Assert.True((actual.CreatedDate - expected).Duration() <= TimeSpan.FromSeconds(1));
        Assert.Null(actual.PublishedDate);
        Assert.Empty(actual.Audience);
    }
}