using Letterbook.Core.Adapters;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IActivityAdapter> ActivityAdapterMock;
    protected Mock<IShareAdapter> ShareAdapter;
    protected Mock<IMessageBusAdapter> MessageBusAdapterMock;

    protected WithMocks()
    {
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        ShareAdapter = new Mock<IShareAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
    }
}