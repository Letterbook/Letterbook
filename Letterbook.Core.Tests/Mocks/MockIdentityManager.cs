using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests.Mocks;

public class MockIdentityManager
{
    public Mock<IUserStore<AccountIdentity>> UserStore { get; set; }
    public IdentityOptions IdentityOptions { get; set; }
    public Mock<IPasswordHasher<AccountIdentity>> PasswordHasher { get; set; }
    public Mock<IUserValidator<AccountIdentity>> UserValidator { get; set; }
    public Mock<IPasswordValidator<AccountIdentity>> PasswordValidator { get; set; }
    public Mock<ILookupNormalizer> LookupNormalizer { get; set; }
    public Mock<IdentityErrorDescriber> IdentityErrorDescriber { get; set; }
    public Mock<IServiceProvider> ServiceProvider { get; set; }
    public Mock<ILogger<UserManager<AccountIdentity>>> Logger { get; set; }

    public MockIdentityManager(Mock<IUserStore<AccountIdentity>>? userStore = null,
        IdentityOptions? identityOptions = null,
        Mock<IPasswordHasher<AccountIdentity>>? passwordHasher = null,
        Mock<IUserValidator<AccountIdentity>>? userValidator = null,
        Mock<IPasswordValidator<AccountIdentity>>? passwordValidator = null,
        Mock<ILookupNormalizer>? lookupNormalizer = null,
        Mock<IdentityErrorDescriber>? identityErrorDescriber = null, 
        Mock<IServiceProvider>? serviceProvider = null,
        Mock<ILogger<UserManager<AccountIdentity>>>? logger = null)
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
    }

    public UserManager<AccountIdentity> Create(IUserStore<AccountIdentity>? userStore = null,
        IdentityOptions? identityOptions = null,
        IPasswordHasher<AccountIdentity>? passwordHasher = null,
        IUserValidator<AccountIdentity>? userValidator = null,
        IPasswordValidator<AccountIdentity>? passwordValidator = null,
        ILookupNormalizer? lookupNormalizer = null,
        IdentityErrorDescriber? identityErrorDescriber = null,
        IServiceProvider? serviceProvider = null,
        ILogger<UserManager<AccountIdentity>>? logger = null)
    {
        return new UserManager<AccountIdentity>(
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