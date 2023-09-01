using Letterbook.Core;
using Letterbook.Core.Models;

namespace Letterbook.Api.Tests.Fakes;

public class FakeAccountService : IAccountService
{
    private readonly List<string> _webFingerQueries = new();
    private Profile _returnValue = new(null);

    public Account? RegisterAccount(string email, string handle)
    {
        throw new NotImplementedException();
    }

    public Account? LookupAccount(string id)
    {
        throw new NotImplementedException();
    }

    public Profile LookupProfile(string queryTarget)
    {
        _webFingerQueries.Add(queryTarget);
        return _returnValue;
    }

    public IEnumerable<Account> FindAccounts(string email)
    {
        throw new NotImplementedException();
    }

    public bool UpdateEmail(string accountId, string email)
    {
        throw new NotImplementedException();
    }

    public bool AddLinkedProfile(string accountId, Profile profile, ProfilePermission permission)
    {
        throw new NotImplementedException();
    }

    public bool UpdateLinkedProfile(string accountId, Profile profile, ProfilePermission permission)
    {
        throw new NotImplementedException();
    }

    public bool RemoveLinkedProfile(string accountId, Profile profile)
    {
        throw new NotImplementedException();
    }

    public void MustHaveBeenAskedToFind(string queryTarget)
    {
        Assert.Contains(queryTarget, _webFingerQueries);
    }

    public void AlwaysReturn(Profile returnValue)
    {
        _returnValue = returnValue;
    }
}