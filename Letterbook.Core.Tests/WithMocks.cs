using Letterbook.Core.Ports;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IFediPort> FediAdapter;
    protected Mock<IActivityPort> ActivityAdapter;
    protected Mock<ISharePort> ShareAdapter;

    protected WithMocks()
    {
        FediAdapter = new Mock<IFediPort>();
        ActivityAdapter = new Mock<IActivityPort>();
        ShareAdapter = new Mock<ISharePort>();
    }
}