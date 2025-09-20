using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests.Mocks;

/// <summary>
/// Provide mocked instances of major Aspnet Core Identity services
/// </summary>
/// <remarks>
/// You may be thinking this is a lot of mocks, and you would be right. Testing around UserManager _sucks_.
/// The goal is to mock the data access portions of UserManager, while still exercising all of its application logic.
/// So, for instance, we can generate and validate real tokens.
/// </remarks>

// For a somewhat longer explanation: UserManager (And LoginManager, for that matter) are weirdly tightly coupled to the
// data access layer (which is to say, the DbContext). Big sections of it _really_ needs to have a database. AND, that db
// needs to be postgres, because we just use postgres features. So, we can't even use in-memory test DBs. Using a real
// postgres database raises test isolation concerns that make it really unappealing. And addressing those isolation concerns
// makes unit tests too slow. As much as is absolutely possible, unit tests should be extremely fast. My target run time for
// the full suite of unit tests is 15 seconds. Beyond that, running tests starts to risk interrupting a developer's train of
// thought. And they must absolutely have perfect isolation. Anything less than perfect isolation between tests introduces
// flaking and escalating maintenance load. Finally, unit tests should be a development aid, not merely a check for
// correctness. These are tests as in science, not merely tests as in evaluation. Tests should help you to explore and
// discover the behavior of the app, and to reproduce specific scenarios.

public class MockIdentityManager
{
	// These really do need to be mocked
	public Mock<IUserStore<Account>> UserStore { get; set; }
	public IServiceCollection ServiceCollection { get; set; }
	public Mock<ILogger<UserManager<Account>>> Logger { get; set; }

	// These might not
	public Mock<IUserValidator<Account>> UserValidator { get; set; }
	public Mock<IPasswordValidator<Account>> PasswordValidator { get; set; }

	// These probably don't
	public Mock<IPasswordHasher<Account>> PasswordHasher { get; set; }
	public Mock<ILookupNormalizer> LookupNormalizer { get; set; }
	public Mock<IdentityErrorDescriber> IdentityErrorDescriber { get; set; }

	public MockIdentityManager(Mock<IUserStore<Account>>? userStore = null,
		Mock<IPasswordHasher<Account>>? passwordHasher = null,
		Mock<IUserValidator<Account>>? userValidator = null,
		Mock<IPasswordValidator<Account>>? passwordValidator = null,
		Mock<ILookupNormalizer>? lookupNormalizer = null,
		Mock<IdentityErrorDescriber>? identityErrorDescriber = null,
		Mock<IUserTwoFactorTokenProvider<Account>>? tokenProvider = null,
		Mock<ILogger<UserManager<Account>>>? logger = null)
	{
		UserStore = userStore ?? new();
		PasswordHasher = passwordHasher ?? new();
		UserValidator = userValidator ?? new();
		PasswordValidator = passwordValidator ?? new();
		LookupNormalizer = lookupNormalizer ?? new();
		IdentityErrorDescriber = identityErrorDescriber ?? new();
		ServiceCollection = new ServiceCollection();
		Logger = logger ?? new();

		ServiceCollection.AddIdentity<Account, IdentityRole<Guid>>(opts => opts.ConfigureIdentity())
			.AddDefaultTokenProviders();
		ServiceCollection.AddLogging();

		UserStore.As<IUserPasswordStore<Account>>();
		UserStore.As<IUserClaimStore<Account>>();
		UserStore.As<IUserEmailStore<Account>>();
		UserStore.As<IUserLoginStore<Account>>();
		UserStore.Setup(m => m.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(IdentityResult.Success);
		PasswordValidator
			.Setup(m => m.ValidateAsync(It.IsAny<UserManager<Account>>(), It.IsAny<Account>(), It.IsAny<string?>()))
			.ReturnsAsync(IdentityResult.Success);
		UserValidator
			.Setup(m => m.ValidateAsync(It.IsAny<UserManager<Account>>(), It.IsAny<Account>()))
			.ReturnsAsync(IdentityResult.Success);
	}

	public UserManager<Account> Create(IUserStore<Account>? userStore = null,
		IPasswordHasher<Account>? passwordHasher = null,
		IUserValidator<Account>? userValidator = null,
		IPasswordValidator<Account>? passwordValidator = null,
		ILookupNormalizer? lookupNormalizer = null,
		IdentityErrorDescriber? identityErrorDescriber = null,
		ILogger<UserManager<Account>>? logger = null)
	{
		var provider = ServiceCollection.BuildServiceProvider();
		var opts = provider.GetRequiredService<IOptions<IdentityOptions>>();
		return new UserManager<Account>(
			userStore ?? UserStore.Object,
			opts,
			passwordHasher ?? PasswordHasher.Object,
			new[] { userValidator ?? UserValidator.Object },
			new[] { passwordValidator ?? PasswordValidator.Object },
			lookupNormalizer ?? LookupNormalizer.Object,
			identityErrorDescriber ?? IdentityErrorDescriber.Object,
			provider,
			logger ?? Logger.Object);
	}
}