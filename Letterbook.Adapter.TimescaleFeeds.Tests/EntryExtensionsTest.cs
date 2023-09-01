using System.Text;
using Letterbook.Adapter.TimescaleFeeds.Extensions;

namespace Letterbook.Adapter.TimescaleFeeds._Tests;

public class EntryExtensionsTest
{
    [Fact]
    public void TestRowTemplate0()
    {
        var sb = new StringBuilder();
        sb.AppendEntryRow(0);
        Assert.Equal("({0},{1},{2},{3},{4},{5},{6},{7},{8})", sb.ToString());
    }
    
    [Fact]
    public void TestRowTemplate1()
    {
        var sb = new StringBuilder();
        sb.AppendEntryRow(1);
        Assert.Equal("({9},{10},{11},{12},{13},{14},{15},{16},{17})", sb.ToString());
    }
}