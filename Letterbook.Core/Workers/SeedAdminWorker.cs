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
			var instance = Profile.SystemInstance(_coreOptions);
			if (!await _data.Profiles(instance.Id).AnyAsync(cancellationToken: cancellationToken))
			{
				_data.Add(instance);
				_logger.LogInformation("Seeding instance profile {Id}", instance.Id);
			}

			var mods = Profile.SystemModerators(_coreOptions);
			if (!await _data.Profiles(mods.Id).AnyAsync(cancellationToken: cancellationToken))
			{
				_data.Add(mods);
				_logger.LogInformation("Seeding moderator profile {Id}", mods.Id);
			}

			await _data.Commit();

			// TODO: replace with invite mechanism
			if (_data.AllAccounts().Any())
			{
				_logger.LogDebug("Found accounts, skipping seed");
				return;
			}
			var admin = await _accountService.RegisterAccount($"admin@letterbook.example", "admin",
				"Password1!");
			if (admin is not null)
			{
				_logger.LogInformation("Created admin account");
				return;
			}

			_logger.LogError("Couldn't create admin account");
		}
		catch (OperationCanceledException ex)
		{
			_logger.LogInformation("Cancelling ({Message})", ex.Message);
		}
	}
}