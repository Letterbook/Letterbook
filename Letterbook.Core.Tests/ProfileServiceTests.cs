using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Extensions;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class ProfileServiceTests : WithMocks
{
    private ITestOutputHelper _output;
    private ProfileService _service;
    private FakeAccount _fakeAccount;
    private FakeProfile _fakeProfile;

    public ProfileServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _fakeAccount = new FakeAccount();
        _fakeProfile = new FakeProfile();
        
        _service = new ProfileService(Mock.Of<ILogger<ProfileService>>(), CoreOptionsMock, AccountProfileMock.Object, Mock.Of<IProfileEventService>());
    }

    [Fact]
    public void ShouldExist()
    {
        Assert.NotNull(_service);
    }

    [Fact(DisplayName = "Should create a new profile")]
    public async Task CreateNewProfile()
    {
        var accountId = Guid.NewGuid();
        var expected = "testAccount";
        AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
        AccountProfileMock.Setup(m => m.QueryProfiles(It.IsAny<IAccountProfileAdapter.ProfileQuery>(), It.IsAny<int?>()))
            .Returns(() => new List<Profile>().AsAsyncEnumerable());
        
        var actual = await _service.CreateProfile(accountId, expected);
        
        Assert.NotNull(actual);
        Assert.Equal($"@{expected}@letterbook.example", actual.Handle);
    }
    
    [Fact(DisplayName = "Should error when trying to create an orphan profile")]
    public async Task NoCreateOrphanProfile()
    {
        var accountId = Guid.NewGuid();
        var expected = "testAccount";
        AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(default (Account));
        AccountProfileMock.Setup(m => m.QueryProfiles(It.IsAny<IAccountProfileAdapter.ProfileQuery>(), It.IsAny<int?>()))
            .Returns(() => new List<Profile>().AsAsyncEnumerable());
        
        await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
    }
    
    [Fact(DisplayName = "Should not create a duplicate profile")]
    public async Task NoCreateDuplicate()
    {
        var accountId = Guid.NewGuid();
        var expected = "testAccount";
        var existing = _fakeProfile.Generate();
        existing.Handle = expected;
        AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
        AccountProfileMock.Setup(m => m.AnyProfile(It.IsAny<IAccountProfileAdapter.ProfileComparer>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
    }
}