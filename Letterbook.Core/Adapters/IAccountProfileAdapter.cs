using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
	delegate IQueryable<Profile> ProfileQuery(IQueryable<Profile> querySource);

	delegate IQueryable<Account> AccountQuery(IQueryable<Account> querySource);

	delegate bool ProfileComparer(Profile profile);

	public bool RecordAccount(Account account);
	public Task<bool> RecordAccounts(IEnumerable<Account> accounts);
	public Task<Account?> LookupAccount(Uuid7 id);
	public IQueryable<Account> SearchAccounts();

	public Task<bool> AnyProfile(string handle);
	public Task<Profile?> LookupProfile(Uuid7 localId);
	public Task<Profile?> LookupProfile(Uri id);

	// Lookup Profile including relations to another profile
	public Task<Profile?> LookupProfileWithRelation(Uri id, Uri relationId);
	public Task<Profile?> LookupProfileWithRelation(Uuid7 localId, Uri relationId);

	public IAsyncEnumerable<Profile> FindProfilesByHandle(string handle, bool partial = false, int limit = 20, int page = 0);

	public void Add(Profile profile);
	public void Add(Account account);
	public void AddRange(IEnumerable<Profile> profile);
	public void AddRange(IEnumerable<Account> account);
	public void Update(Profile profile);
	public void Update(Account account);
	public void UpdateRange(IEnumerable<Profile> profile);
	public void UpdateRange(IEnumerable<Account> account);
	public void Delete(object record);

	public Task Cancel();
	public Task Commit();
}