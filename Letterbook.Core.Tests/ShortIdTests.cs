using Letterbook.Core.Extensions;

namespace Letterbook.Core.Tests;


public class ShortIdTests
{
    public static TheoryData<Guid> GuidList(int count)
    {
        var data = new TheoryData<Guid>();
        for (var i = 0; i < count; i++)
        {
            data.Add(Guid.NewGuid());
        }

        return data;
    }

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