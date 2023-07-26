using Letterbook.Core.Adapters;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IFediAdapter> FediAdapterMock;
    protected Mock<IActivityAdapter> ActivityAdapterMock;
    protected Mock<IShareAdapter> ShareAdapter;
    protected Mock<IMessageBusAdapter> MessageBusAdapterMock;

    protected WithMocks()
    {
        FediAdapterMock = new Mock<IFediAdapter>();
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        ShareAdapter = new Mock<IShareAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
    }
}