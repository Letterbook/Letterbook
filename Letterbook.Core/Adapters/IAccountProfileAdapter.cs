using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
    delegate IQueryable<Profile> ProfileQuery(IQueryable<Profile> querySource);

    delegate IQueryable<Account> AccountQuery(IQueryable<Account> querySource);

    delegate bool ProfileComparer(Profile profile);

    public bool RecordAccount(Account account);
    public Task<bool> RecordAccounts(IEnumerable<Account> accounts);
    public Task<Account?> LookupAccount(Guid id);
    public IQueryable<Account> SearchAccounts();

    public Task<bool> AnyProfile(ProfileComparer comparer);
    public Task<Profile?> LookupProfile(Guid localId);
    public Task<Profile?> LookupProfile(Uri id);
    
    // Lookup Profile with related properties matching the following ID
    public Task<Profile?> LookupProfileForFollowing(Uri id, Uri followingId);
    public Task<Profile?> LookupProfileForFollowing(Guid localId, Uri followingId);
    
    // Lookup Profile with related properties matching the follower ID
    public Task<Profile?> LookupProfileForFollowers(Uri localId, Uri followerId);
    public Task<Profile?> LookupProfileForFollowers(Guid localId, Uri followerId);
    public IAsyncEnumerable<Profile> QueryProfiles(ProfileQuery query, int? limit = null);

    public bool RecordProfile(Profile profile);

    public void Delete(object record);
    public Task Cancel();
    public Task Commit();
}