using Letterbook.Core.Adapters;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IFediAdapter> FediAdapter;
    protected Mock<IActivityAdapter> ActivityAdapter;
    protected Mock<IShareAdapter> ShareAdapter;

    protected WithMocks()
    {
        FediAdapter = new Mock<IFediAdapter>();
        ActivityAdapter = new Mock<IActivityAdapter>();
        ShareAdapter = new Mock<IShareAdapter>();
    }
}