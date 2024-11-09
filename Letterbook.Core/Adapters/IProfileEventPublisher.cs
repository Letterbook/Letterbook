using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IProfileEventPublisher
{
	public Task Created(Profile profile);
	public Task Deleted(Profile profile);
	public Task Updated(Profile original, Profile updated);
	public Task Migrated(Profile profile, Profile migratedFrom);
	public Task Reported(Profile profile, Profile? reportedBy = default);
	public Task Blocked(Profile profile, Profile blockedBy); // ?
	public Task Followed(Profile profile, Profile followedBy);
	public Task Unfollowed(Profile profile, Profile unfollowedBy);
}