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
	private readonly IInviteCodeGenerator _inviteCodeGenerator;

	public SeedAdminWorker(ILogger<SeedAdminWorker> logger, IOptions<CoreOptions> coreOpts,
		IAccountService accountService, IDataAdapter data, IInviteCodeGenerator inviteCodeGenerator)
	{
		_logger = logger;
		_coreOptions = coreOpts.Value;
		_accountService = accountService;
		_data = data;
		_inviteCodeGenerator = inviteCodeGenerator;
	}

	public async Task DoWork(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (await _data.AllAccounts().AnyAsync(cancellationToken: cancellationToken))
			{
				_logger.LogDebug("Found existing accounts, skipping seed");
			}
			else if (await _data.AllInviteCodes().AnyAsync(cancellationToken: cancellationToken))
			{
				_logger.LogDebug("Found existing invitations, skipping seed");
			}
			else
			{

				var invite = new InviteCode(_inviteCodeGenerator) { Uses = 1 };
				_data.Add(invite);
				try
				{
					await _data.Commit();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Couldn't seed initial invite code");
					throw;
				}

				_logger.LogInformation("Welcome to Letterbook!");
				_logger.LogInformation("Seeding database with initial data");
				_logger.LogWarning("Letterbook does not have a default admin account. You must create one yourself. This is by design");
				_logger.LogCritical("\n*****\nThis is a single-use invite code. Use this code ({Code}) to create an account on the site\n*****", invite.Code);
			}

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