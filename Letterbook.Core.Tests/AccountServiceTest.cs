using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class AccountServiceTest : WithMocks
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly AccountService _accountService;
    private int _randomSeed;
    private FakeAccount _fakeAccount;
    private FakeProfile _fakeProfile;

    public AccountServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        var loggerMock = Mock.Of<ILogger<AccountService>>();
        _fakeAccount = new FakeAccount();
        _fakeProfile = new FakeProfile();

        _accountService = new AccountService(loggerMock, CoreOptionsMock, AccountProfileMock.Object,
            AccountEventServiceMock.Object);
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_accountService);
    }

    [Fact(DisplayName = "Should Register new accounts")]
    public void RegisterAccount()
    {
        AccountProfileMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);
        
        var actual = _accountService.RegisterAccount("test@example.com", "tester");

        Assert.IsType<Account>(actual);
    }
    
    [Fact(DisplayName = "Should publish new accounts")]
    public void RegisterAccountPublish()
    {
        AccountProfileMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);
        
        _accountService.RegisterAccount("test@example.com", "tester");

        AccountEventServiceMock.Verify(mock => mock.Created(It.IsAny<Account>()));
    }
    
    [Fact(DisplayName = "Should do nothing when registration fails")]
    public void RegisterAccountFail()
    {
        AccountProfileMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(false);
        
        var actual = _accountService.RegisterAccount("test@example.com", "tester");

        Assert.Null(actual);
        AccountEventServiceMock.VerifyNoOtherCalls();
    }
}