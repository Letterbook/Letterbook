using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Mocks;
using Medo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;


public class AccountServiceTest : WithMocks
{
	private readonly ITestOutputHelper _outputHelper;
	private readonly AccountService _accountService;
	private FakeAccount _fakeAccount;
	private FakeProfile _fakeProfile;
	private MockIdentityManager _mockIdentityManager;

	public AccountServiceTest(ITestOutputHelper outputHelper)
	{
		_outputHelper = outputHelper;
		_outputHelper.WriteLine($"Bogus random seed: {Init.WithSeed()}");
		var loggerMock = Mock.Of<ILogger<AccountService>>();
		_fakeAccount = new FakeAccount();
		_fakeProfile = new FakeProfile();
		_mockIdentityManager = new MockIdentityManager();

		_accountService = new AccountService(loggerMock, CoreOptionsMock, AccountProfileMock.Object,
			AccountEventServiceMock.Object, _mockIdentityManager.Create());
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_accountService);
	}

	[Fact(DisplayName = "Should Register new accounts")]
	public async Task RegisterAccount()
	{
		AccountProfileMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);

		var actual = await _accountService.RegisterAccount("test@example.com", "tester", "password");

		Assert.NotNull(actual);
		Assert.True(actual.Succeeded);
	}

	[Fact(DisplayName = "Should publish new accounts")]
	public async Task RegisterAccountPublish()
	{
		AccountProfileMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);

		await _accountService.RegisterAccount("test@example.com", "tester", "password");

		AccountEventServiceMock.Verify(mock => mock.Created(It.IsAny<Account>()));
	}

	[Fact(DisplayName = "Should do nothing when registration fails")]
	public async Task RegisterAccountFail()
	{
		_mockIdentityManager.UserStore.Setup(m => m.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(IdentityResult.Failed());

		var actual = await _accountService.RegisterAccount("test@example.com", "tester", "password");

		Assert.False(actual.Succeeded);
		AccountEventServiceMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should return single lookups")]
	public async Task LookupTest()
	{
		var expected = _fakeAccount.Generate();
		AccountProfileMock.Setup(m => m.LookupAccount(It.IsAny<Uuid7>())).ReturnsAsync(expected);

		var actual = await _accountService.LookupAccount(expected.Id);

		Assert.Equal(expected, actual);
	}

	[Fact(DisplayName = "Lookups should return nothing when no account is found")]
	public async Task LookupTestNoResult()
	{
		var expected = _fakeAccount.Generate();
		AccountProfileMock.Setup(m => m.LookupAccount(It.IsAny<Uuid7>())).ReturnsAsync(default(Account));

		var actual = await _accountService.LookupAccount(expected.Id);

		Assert.NotEqual(expected, actual);
		Assert.Null(actual);
	}

	[Fact(DisplayName = "Should update the email address")]
	public async Task UpdateTest()
	{
		var account = _fakeAccount.Generate();
		AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.UpdateEmail(account.Id, "test@example.com");

		Assert.Equal("test@example.com", account.Email);
	}

	[Fact(DisplayName = "Should link new profiles")]
	public async Task AddLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileAccess(account, profile, ProfilePermission.All);
		AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.AddLinkedProfile(account.Id, profile, ProfilePermission.All);

		Assert.Contains(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
	}

	[Fact(DisplayName = "Should remove linked profiles")]
	public async Task RemoveLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileAccess(account, profile, ProfilePermission.All);
		account.LinkedProfiles.Add(expected);
		AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.RemoveLinkedProfile(account.Id, profile);

		Assert.DoesNotContain(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
	}

	[Fact(DisplayName = "Should update profile permissions")]
	public async Task UpdateLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileAccess(account, profile, ProfilePermission.All);
		account.LinkedProfiles.Add(expected);
		AccountProfileMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.UpdateLinkedProfile(account.Id, profile, ProfilePermission.None);

		var accountLink = account.LinkedProfiles.SingleOrDefault(p => p.Equals(expected));
		Assert.Equal(accountLink?.Permission, ProfilePermission.None);
	}
}