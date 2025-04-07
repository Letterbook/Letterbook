using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Workers;

/// <summary>
/// Seeds the database with system profiles
/// </summary>
public class SeedAdminWorker : IScopedWorker
{
	private readonly ILogger<SeedAdminWorker> _logger;
	private readonly CoreOptions _coreOptions;
	private readonly IAccountService _accountService;
	private readonly IDataAdapter _data;

	public SeedAdminWorker(ILogger<SeedAdminWorker> logger, IOptions<CoreOptions> coreOpts,
		IAccountService accountService, IDataAdapter data)
	{
		_logger = logger;
		_coreOptions = coreOpts.Value;
		_accountService = accountService;
		_data = data;
	}

	public async Task DoWork(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			// TODO: replace with invite mechanism
			if (_data.AllAccounts().Any())
			{
				_logger.LogDebug("Found accounts, skipping seed");
				return;
			}
			var admin = await _accountService.RegisterAccount($"admin@letterbook.example", "admin",
				"Password1!");
			if (admin is not null) _logger.LogInformation("Created admin account");
			else _logger.LogError("Couldn't create admin account");

			if (await _data.Profiles(Profile.SystemInstanceId)
				    .AsNoTracking()
				    .FirstOrDefaultAsync(cancellationToken: cancellationToken) is { } instance)
			{
				Profile.AddInstanceProfile(instance);
			}
			else {
				_data.Add(Profile.GetOrAddInstanceProfile(_coreOptions));
				_logger.LogInformation("Seeding instance profile {Id}", Profile.SystemInstanceId);
			}

			if (await _data.Profiles(Profile.SystemModeratorsId)
				    .AsNoTracking()
				    .FirstOrDefaultAsync(cancellationToken: cancellationToken) is { } moderators)
			{
				Profile.AddModeratorsProfile(moderators);
			}
			else {
				_data.Add(Profile.GetOrAddModeratorsProfile(_coreOptions));
				_logger.LogInformation("Seeding moderator profile {Id}", Profile.SystemModeratorsId);
			}

			await _data.Commit();
		}
		catch (OperationCanceledException ex)
		{
			_logger.LogInformation("Cancelling ({Message})", ex.Message);
		}
	}
}