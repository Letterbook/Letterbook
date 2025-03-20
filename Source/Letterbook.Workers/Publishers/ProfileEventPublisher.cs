using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Workers.Publishers;

public class ProfileEventPublisher : IProfileEventPublisher
{
	private readonly ILogger<ProfileEventPublisher> _logger;

	public ProfileEventPublisher(ILogger<ProfileEventPublisher> logger)
	{
		_logger = logger;
	}

	public Task Created(Profile profile)
	{
		_logger.LogWarning($"{nameof(Created)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Deleted(Profile profile)
	{
		_logger.LogWarning($"{nameof(Deleted)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Updated(Profile original, Profile updated)
	{
		_logger.LogWarning($"{nameof(updated)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Migrated(Profile profile, Profile migratedFrom)
	{
		_logger.LogWarning($"{nameof(Migrated)} event not implemented");
		return Task.CompletedTask;
	}

	public Task MigratedOut(Profile profile)
	{
		_logger.LogWarning($"{nameof(MigratedOut)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Reported(Profile profile, Profile? reportedBy = default)
	{
		_logger.LogWarning($"{nameof(Reported)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Blocked(Profile profile, Profile blockedBy)
	{
		_logger.LogWarning($"{nameof(Blocked)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Followed(Profile profile, Profile followedBy)
	{
		_logger.LogWarning($"{nameof(Followed)} event not implemented");
		return Task.CompletedTask;
	}

	public Task Unfollowed(Profile profile, Profile unfollowedBy)
	{
		_logger.LogWarning($"{nameof(Unfollowed)} event not implemented");
		return Task.CompletedTask;
	}
}