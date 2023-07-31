using Letterbook.Core.Adapters;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
    protected Mock<IActivityAdapter> ActivityAdapterMock;
    protected Mock<IShareAdapter> ShareAdapter;
    protected Mock<IMessageBusAdapter> MessageBusAdapterMock;
    protected IOptions<CoreOptions> CoreOptionsMock;

    protected WithMocks()
    {
        ActivityAdapterMock = new Mock<IActivityAdapter>();
        ShareAdapter = new Mock<IShareAdapter>();
        MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
        var mockOptions = new CoreOptions
        {
            DomainName = "letterbook.example",
            Port = "80",
            Scheme = "http"
        };
        CoreOptionsMock = Options.Create(mockOptions);
    }
}