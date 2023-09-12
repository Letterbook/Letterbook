using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests.Mocks;

public class MockIdentityManager
{
    public Mock<IUserStore<Account>> UserStore { get; set; }
    public IdentityOptions IdentityOptions { get; set; }
    public Mock<IPasswordHasher<Account>> PasswordHasher { get; set; }
    public Mock<IUserValidator<Account>> UserValidator { get; set; }
    public Mock<IPasswordValidator<Account>> PasswordValidator { get; set; }
    public Mock<ILookupNormalizer> LookupNormalizer { get; set; }
    public Mock<IdentityErrorDescriber> IdentityErrorDescriber { get; set; }
    public Mock<IServiceProvider> ServiceProvider { get; set; }
    public Mock<ILogger<UserManager<Account>>> Logger { get; set; }

    public MockIdentityManager(Mock<IUserStore<Account>>? userStore = null,
        IdentityOptions? identityOptions = null,
        Mock<IPasswordHasher<Account>>? passwordHasher = null,
        Mock<IUserValidator<Account>>? userValidator = null,
        Mock<IPasswordValidator<Account>>? passwordValidator = null,
        Mock<ILookupNormalizer>? lookupNormalizer = null,
        Mock<IdentityErrorDescriber>? identityErrorDescriber = null, 
        Mock<IServiceProvider>? serviceProvider = null,
        Mock<ILogger<UserManager<Account>>>? logger = null)
    {
        UserStore = userStore ?? new();
        IdentityOptions = identityOptions ?? new();
        PasswordHasher = passwordHasher ?? new();
        UserValidator = userValidator ?? new();
        PasswordValidator = passwordValidator ?? new();
        LookupNormalizer = lookupNormalizer ?? new();
        IdentityErrorDescriber = identityErrorDescriber ?? new();
        ServiceProvider = serviceProvider ?? new();
        Logger = logger ?? new();
        
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
        IdentityOptions? identityOptions = null,
        IPasswordHasher<Account>? passwordHasher = null,
        IUserValidator<Account>? userValidator = null,
        IPasswordValidator<Account>? passwordValidator = null,
        ILookupNormalizer? lookupNormalizer = null,
        IdentityErrorDescriber? identityErrorDescriber = null,
        IServiceProvider? serviceProvider = null,
        ILogger<UserManager<Account>>? logger = null)
    {
        return new UserManager<Account>(
            userStore ?? UserStore.Object,
            Options.Create(identityOptions ?? IdentityOptions),
            passwordHasher ?? PasswordHasher.Object,
            new[] { userValidator ?? UserValidator.Object },
            new[] { passwordValidator ?? PasswordValidator.Object },
            lookupNormalizer ?? LookupNormalizer.Object,
            identityErrorDescriber ?? IdentityErrorDescriber.Object,
            serviceProvider ?? ServiceProvider.Object,
            logger ?? Logger.Object);
    }
}