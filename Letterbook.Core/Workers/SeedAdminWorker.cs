using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Workers;

/// <summary>
/// This should eventually seed the admin invite on first launch. For now it just creates an account, as a convenience
/// during development
/// </summary>
public class SeedAdminWorker : IScopedWorker
{
	private readonly ILogger<SeedAdminWorker> _logger;
	private readonly CoreOptions _coreOptions;
	private readonly IAccountService _accountService;
	private readonly IDataAdapter _accountAdapter;

	public SeedAdminWorker(ILogger<SeedAdminWorker> logger, IOptions<CoreOptions> coreOpts,
		IAccountService accountService, IDataAdapter accountAdapter)
	{
		_logger = logger;
		_coreOptions = coreOpts.Value;
		_accountService = accountService;
		_accountAdapter = accountAdapter;
	}

	public async Task DoWork(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (_accountAdapter.AllAccounts().Any())
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