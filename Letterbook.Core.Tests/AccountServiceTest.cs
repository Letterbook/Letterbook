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

    [Fact(DisplayName = "Should return single lookups")]
    public void LookupTest()
    {
        var expected = _fakeAccount.Generate();
        AccountProfileMock.Setup(m => m.LookupAccount(It.IsAny<string>())).Returns(expected);

        var actual = _accountService.LookupAccount(expected.Id);

        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "Lookups should return nothing when no account is found")]
    public void LookupTestNoResult()
    {
        var expected = _fakeAccount.Generate();
        AccountProfileMock.Setup(m => m.LookupAccount(It.IsAny<string>())).Returns(default(Account));

        var actual = _accountService.LookupAccount(expected.Id);

        Assert.NotEqual(expected, actual);
        Assert.Null(actual);
    }

    [Fact(DisplayName = "Should update the email address")]
    public void UpdateTest()
    {
        var account = _fakeAccount.Generate();
        AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).Returns(account);

        _accountService.UpdateEmail(account.Id, "test@example.com");

        Assert.Equal("test@example.com", account.Email);
    }

    [Fact(DisplayName = "Should link new profiles")]
    public void AddLinkedProfile()
    {
        var account = _fakeAccount.Generate();
        var profile = _fakeProfile.Generate();
        var expected = new LinkedProfile(account, profile, ProfilePermission.All);
        AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).Returns(account);

        _accountService.AddLinkedProfile(account.Id, profile, ProfilePermission.All);
        
        Assert.Contains(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
        Assert.Contains(profile.RelatedAccounts, linkedProfile => linkedProfile == expected);
    }
    
    [Fact(DisplayName = "Should remove linked profiles")]
    public void RemoveLinkedProfile()
    {
        var account = _fakeAccount.Generate();
        var profile = _fakeProfile.Generate();
        var expected = new LinkedProfile(account, profile, ProfilePermission.All);
        account.LinkedProfiles.Add(expected);
        profile.RelatedAccounts.Add(expected);
        AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).Returns(account);

        _accountService.RemoveLinkedProfile(account.Id, profile);
        
        Assert.DoesNotContain(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
        Assert.DoesNotContain(profile.RelatedAccounts, linkedProfile => linkedProfile == expected);
    }
    
    [Fact(DisplayName = "Should update profile permissions")]
    public void UpdateLinkedProfile()
    {
        var account = _fakeAccount.Generate();
        var profile = _fakeProfile.Generate();
        var expected = new LinkedProfile(account, profile, ProfilePermission.All);
        account.LinkedProfiles.Add(expected);
        profile.RelatedAccounts.Add(expected);
        AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).Returns(account);

        _accountService.UpdateLinkedProfile(account.Id, profile, ProfilePermission.None);
        
        var profileLink = profile.RelatedAccounts.SingleOrDefault(p => p.Equals(expected));
        var accountLink = account.LinkedProfiles.SingleOrDefault(p => p.Equals(expected));
        Assert.Equal(profileLink?.Permission, ProfilePermission.None);
        Assert.Equal(accountLink?.Permission, ProfilePermission.None);
    }
}