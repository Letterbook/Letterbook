using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Workers.Publishers;

public class ModerationEventPublisher : IModerationEventPublisher
{
	private readonly ILogger<ModerationEventPublisher> _logger;

	public ModerationEventPublisher(ILogger<ModerationEventPublisher> logger)
	{
		_logger = logger;
	}

	public Task Created(ModerationReport report, ProfileId author, IEnumerable<Claim> claims)
	{
		_logger.LogWarning($"{nameof(Created)} event not implemented");
		return Task.CompletedTask;
	}
	public Task Forwarded(ModerationReport report, IEnumerable<Uri> inboxes, IEnumerable<Claim> claims)
	{
		_logger.LogWarning($"{nameof(Forwarded)} event not implemented");
		return Task.CompletedTask;
	}
	public Task Assigned(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		_logger.LogWarning($"{nameof(Assigned)} event not implemented");
		return Task.CompletedTask;
	}
	public Task Closed(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		_logger.LogWarning($"{nameof(Closed)} event not implemented");
		return Task.CompletedTask;
	}
	public Task Reopened(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		_logger.LogWarning($"{nameof(Reopened)} event not implemented");
		return Task.CompletedTask;
	}
}