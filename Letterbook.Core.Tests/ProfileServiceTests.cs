using Bogus;
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
    private Profile _profile;

    public ProfileServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _fakeAccount = new FakeAccount();
        _fakeProfile = new FakeProfile("letterbook.example");
        var genericFaker = new Faker();
        CoreOptionsMock.Value.MaxCustomFields = 2;

        _service = new ProfileService(Mock.Of<ILogger<ProfileService>>(), CoreOptionsMock, AccountProfileMock.Object,
            Mock.Of<IProfileEventService>(), Mock.Of<IActivityPubClient>());
        _profile = _fakeProfile.Generate();
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
    
    [Fact(DisplayName = "Should not create an orphan profile")]
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

    [Fact(DisplayName = "Should update the display name")]
    public async Task UpdateDisplayName()
    {
        var expectedId = Guid.NewGuid();
        _profile.LocalId = expectedId;
        _profile.DisplayName = new Faker().Internet.UserName();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == expectedId)))
            .ReturnsAsync(_profile);

        var actual = await _service.UpdateDisplayName(expectedId, "Test Name");
        
        // Assert.NotEqual(_profile.LocalId, expectedId);
        Assert.NotNull(actual.Updated);
        Assert.Equal("Test Name", actual.Updated.DisplayName);
    }
    
    [Fact(DisplayName = "Should not update the display name when the profile doesn't exist")]
    public async Task NoUpdateDisplayNameNotExists()
    {
        var expectedId = Guid.NewGuid();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == expectedId)))
            .ReturnsAsync(default (Profile));

        await Assert.ThrowsAsync<CoreException>(() => _service.UpdateDisplayName(expectedId, "Test Name"));
    }
    
    [Fact(DisplayName = "Should not update the display name when the name is unchanged")]
    public async Task NoUpdateDisplayNameUnchanged()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.UpdateDisplayName((Guid)_profile.LocalId!, _profile.DisplayName);
        
        Assert.Null(actual.Updated);
        Assert.Equal(_profile, actual.Original);
        Assert.Equal(_profile.DisplayName, actual.Original.DisplayName);
        Assert.NotNull(actual.Original.DisplayName);
    }
    
    [Fact(DisplayName = "Should update the bio")]
    public async Task UpdateBio()
    {
        var expectedId = Guid.NewGuid();
        _profile.LocalId = expectedId;
        _profile.DisplayName = new Faker().Internet.UserName();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == expectedId)))
            .ReturnsAsync(_profile);

        var actual = await _service.UpdateDescription(expectedId, "This is a test user bio");
        
        Assert.NotNull(actual.Updated);
        Assert.Equal("This is a test user bio", actual.Updated.Description);
    }
    
    [Fact(DisplayName = "Should not update the bio when the profile doesn't exist")]
    public async Task NoUpdateBioNotExists()
    {
        var expectedId = Guid.NewGuid();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == expectedId)))
            .ReturnsAsync(default (Profile));

        await Assert.ThrowsAsync<CoreException>(() => _service.UpdateDescription(expectedId, "This is a test user bio"));
    }
    
    [Fact(DisplayName = "Should not update the bio when it is unchanged")]
    public async Task NoUpdateBioUnchanged()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.UpdateDescription((Guid)_profile.LocalId!, _profile.Description);
        
        Assert.Null(actual.Updated);
        Assert.Equal(_profile, actual.Original);
        Assert.Equal(_profile.Description, actual.Original.Description);
        Assert.NotNull(actual.Original.Description);
    }
    
    [Fact(DisplayName = "Should insert new custom fields")]
    public async Task InsertCustomField()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.InsertCustomField((Guid)_profile.LocalId!, 0, "test item", "test value");
        // var (original, actual) = await _service.InsertCustomField((Guid)_profile.LocalId!, 0, "test item", "test value");
        
        // Assert.NotEqual(_profile.LocalId, expectedId);
        Assert.NotNull(actual.Updated);
        Assert.Equal("test item", actual.Updated.CustomFields[0].Label);
        Assert.Equal("test value", actual.Updated.CustomFields[0].Value);
        Assert.NotEqual("test value", actual.Original.CustomFields[0].Value);
        Assert.NotEqual("test item", actual.Original.CustomFields[0].Label);
        Assert.Equal(2, actual.Updated.CustomFields.Length);
    }
    
    [Fact(DisplayName = "Should insert new custom fields at given index")]
    public async Task InsertCustomFieldAtIndex()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.InsertCustomField((Guid)_profile.LocalId!, 1, "test item", "test value");
        // var (original, actual) = await _service.InsertCustomField((Guid)_profile.LocalId!, 1, "test item", "test value");
        
        // Assert.NotEqual(_profile.LocalId, expectedId);
        Assert.NotNull(actual.Updated);
        Assert.Equal("test item", actual.Updated.CustomFields[1].Label);
        Assert.Equal("test value", actual.Updated.CustomFields[1].Value);
        Assert.NotEqual(actual.Updated.CustomFields[1].Label, actual.Original.CustomFields[0].Value);
        Assert.NotEqual(actual.Updated.CustomFields[1].Value, actual.Original.CustomFields[0].Label);
        Assert.Equal(2, actual.Updated.CustomFields.Length);
    }
    
    [Fact(DisplayName = "Should not insert custom fields when the profile doesn't exist")]
    public async Task NoInsertCustomField()
    {
        var expectedId = Guid.NewGuid();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == expectedId)))
            .ReturnsAsync(default (Profile));

        await Assert.ThrowsAsync<CoreException>(() => _service.InsertCustomField((Guid)_profile.LocalId!, 0, "test item", "test value"));
    }
    
    [Fact(DisplayName = "Should not insert custom fields when the list is already full")]
    public async Task NoInsertCustomFieldTooMany()
    {
        _profile.CustomFields = _profile.CustomFields.Append(new() { Label = "item2", Value = "value2" }).ToArray();
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        await Assert.ThrowsAsync<CoreException>(() => _service.InsertCustomField((Guid)_profile.LocalId!, 0, "test item", "test value"));
    }
    
    [Fact(DisplayName = "Should update custom fields")]
    public async Task UpdateCustomField()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.UpdateCustomField((Guid)_profile.LocalId!, 0, "test item", "test value");
        // var (_, actual) = await _service.UpdateCustomField((Guid)_profile.LocalId!, 0, "test item", "test value");
        
        Assert.NotNull(actual.Updated);
        Assert.Equal("test item", actual.Updated.CustomFields[0].Label);
        Assert.Equal("test value", actual.Updated.CustomFields[0].Value);
        Assert.Single(actual.Updated.CustomFields);
    }
    
    [Fact(DisplayName = "Should delete custom fields")]
    public async Task DeleteCustomField()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Guid>(given => given == _profile.LocalId)))
            .ReturnsAsync(_profile);

        var actual = await _service.RemoveCustomField((Guid)_profile.LocalId!, 0);
        
        Assert.NotNull(actual.Updated);
        Assert.Empty(actual.Updated.CustomFields);
    }
}