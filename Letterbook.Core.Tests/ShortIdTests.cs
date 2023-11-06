using Letterbook.Core.Extensions;

namespace Letterbook.Core.Tests;


public class ShortIdTests
{
    public static IEnumerable<object[]> GuidList(int count) =>
        Enumerable.Range(0, count).Select(_ => new object[] { Guid.NewGuid() });

    [Theory]
    [MemberData(nameof(GuidList), 10)]
    public void Convert(Guid expected)
    {
        var shortId = expected.ToShortId();
        var actual = ShortId.ToGuid(shortId);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewShortId()
    {
        var actual = ShortId.ToGuid(ShortId.NewShortId());

        Assert.NotEqual(Guid.Empty, actual);
    }
}