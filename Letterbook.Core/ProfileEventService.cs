using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core;

public class ProfileEventService : IProfileEventService
{
	private readonly ILogger<ProfileEventService> _logger;

	public ProfileEventService(ILogger<ProfileEventService> logger)
	{
		_logger = logger;
	}

	public void Created(Profile profile)
	{
		_logger.LogWarning($"{nameof(Created)} event not implemented");
	}

	public void Deleted(Profile profile)
	{
		_logger.LogWarning($"{nameof(Deleted)} event not implemented");
	}

	public void Updated(Profile original, Profile updated)
	{
		_logger.LogWarning($"{nameof(updated)} event not implemented");
	}

	public void MigratedIn(Profile profile)
	{
		_logger.LogWarning($"{nameof(MigratedIn)} event not implemented");
	}

	public void MigratedOut(Profile profile)
	{
		_logger.LogWarning($"{nameof(MigratedOut)} event not implemented");
	}

	public void Reported(Profile profile)
	{
		_logger.LogWarning($"{nameof(Reported)} event not implemented");
	}

	public void Blocked(Profile profile)
	{
		_logger.LogWarning($"{nameof(Blocked)} event not implemented");
	}
}