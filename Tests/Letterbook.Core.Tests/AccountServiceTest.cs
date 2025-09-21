using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
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
	private readonly Account _account;

	public AccountServiceTest(ITestOutputHelper outputHelper)
	{
		_outputHelper = outputHelper;
		_outputHelper.WriteLine($"Bogus random seed: {Init.WithSeed()}");
		var loggerMock = Mock.Of<ILogger<AccountService>>();
		_fakeAccount = new FakeAccount();
		_fakeProfile = new FakeProfile();
		_mockIdentityManager = new MockIdentityManager();

		_accountService = new AccountService(loggerMock, CoreOptionsMock, DataAdapterMock.Object,
			AccountEventServiceMock.Object, _mockIdentityManager.Create());

		_account = _fakeAccount.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_accountService);
	}

	[Fact(DisplayName = "Should Register new accounts")]
	public async Task RegisterAccount()
	{
		DataAdapterMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);

		var actual = await _accountService.RegisterAccount("test@example.com", "tester", "password");

		Assert.NotNull(actual);
		Assert.True(actual.Succeeded);
	}

	[Fact(DisplayName = "Should publish new accounts")]
	public async Task RegisterAccountPublish()
	{
		DataAdapterMock.Setup(m => m.RecordAccount(It.IsAny<Account>())).Returns(true);

		await _accountService.RegisterAccount("test@example.com", "tester", "password");

		AccountEventServiceMock.Verify(mock => mock.Created(It.IsAny<Account>()));
	}

	[Fact(DisplayName = "Should return single lookups")]
	public async Task LookupTest()
	{
		var expected = _fakeAccount.Generate();
		var queryable = new List<Account>() { expected }.BuildMock();
		DataAdapterMock.Setup(m => m.AllAccounts()).Returns(queryable);

		var actual = await _accountService.LookupAccount(expected.Id);

		Assert.Equal(expected, actual);
	}

	[Fact(DisplayName = "Lookups should return nothing when no account is found")]
	public async Task LookupTestNoResult()
	{
		var expected = _fakeAccount.Generate();
		var queryable = Array.Empty<Account>().BuildMock();
		DataAdapterMock.Setup(m => m.AllAccounts()).Returns(queryable);

		var actual = await _accountService.LookupAccount(expected.Id);

		Assert.NotEqual(expected, actual);
		Assert.Null(actual);
	}

	[Fact(DisplayName = "Should update the email address")]
	public async Task UpdateEmailTest()
	{
		var account = _fakeAccount.Generate();
		var oldEmail = account.Email!;
		var expected = "test@example.com";

		_mockIdentityManager.UserStore.Setup(m => m.GetUserIdAsync(account, It.IsAny<CancellationToken>())).ReturnsAsync(account.Id.ToString);
		_mockIdentityManager.UserStore.As<IUserEmailStore<Account>>()
			.Setup(m => m.FindByEmailAsync(oldEmail.ToUpper(), It.IsAny<CancellationToken>())).ReturnsAsync(account);
		_mockIdentityManager.UserStore.Setup(m => m.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(IdentityResult.Success)
			.Verifiable();
		DataAdapterMock.Setup(m => m.Accounts(account.Id)).Returns(() => new List<Account>{account}.BuildMock());
		DataAdapterMock.Setup(m => m.AllAccounts()).Returns(() => new List<Account>{account}.BuildMock());
		DataAdapterMock.Setup(m => m.FindAccountByEmail(account.Email!)).ReturnsAsync(account);

		var token = await _accountService.GenerateChangeEmailToken(account.Id, expected);
		var result = await _accountService.ChangeEmailWithToken(oldEmail!, expected, token);

		Assert.True(result.Succeeded);
		_mockIdentityManager.UserStore.Verify();
	}

	[Fact(DisplayName = "Should link new profiles")]
	public async Task AddLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileClaims(account, profile, [ProfileClaim.Owner]);
		DataAdapterMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.AddLinkedProfile(account.Id, profile, [ProfileClaim.Owner]);

		Assert.Contains(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
	}

	[Fact(DisplayName = "Should remove linked profiles")]
	public async Task RemoveLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileClaims(account, profile, [ProfileClaim.Owner]);
		account.LinkedProfiles.Add(expected);
		DataAdapterMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.RemoveLinkedProfile(account.Id, profile);

		Assert.DoesNotContain(account.LinkedProfiles, linkedProfile => linkedProfile == expected);
	}

	[Fact(DisplayName = "Should update profile permissions")]
	public async Task UpdateLinkedProfile()
	{
		var account = _fakeAccount.Generate();
		var profile = _fakeProfile.Generate();
		var expected = new ProfileClaims(account, profile, [ProfileClaim.Owner]);
		account.LinkedProfiles.Add(expected);
		DataAdapterMock.Setup(m => m.LookupAccount(account.Id)).ReturnsAsync(account);

		await _accountService.UpdateLinkedProfile(account.Id, profile, [ProfileClaim.None]);

		var accountLink = account.LinkedProfiles.SingleOrDefault(p => p.Equals(expected));
		Assert.Equal(accountLink?.Claims, [ProfileClaim.None]);
	}

	[Fact(DisplayName = "Should include linked profiles")]
	public async Task CanGetFirstWithProfiles()
	{
		var queryable = new List<Account> { _account }.BuildMock();
		DataAdapterMock.Setup(m => m.AllAccounts())
			.Returns(queryable);

		var actual = await _accountService.FirstAccount(_account.Email!);

		Assert.NotNull(actual);
		Assert.NotNull(actual.LinkedProfiles);
		Assert.NotEmpty(actual.LinkedProfiles);
	}

	[Fact(DisplayName = "Should cast to claims")]
	public async Task CanGetProfileClaims()
	{
		var queryable = new List<Account> { _account }.BuildMock();
		DataAdapterMock.Setup(m => m.AllAccounts())
			.Returns(queryable);

		var result = await _accountService.FirstAccount(_account.Email!);
		Assert.NotNull(result);

		var actual = result.ProfileClaims();
		Assert.NotEmpty(actual);
	}
}