using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Events relating to <see cref="Letterbook.Core.Models.ModerationReport">Moderation Reports</see>
/// </summary>
public interface IModerationEventPublisher
{
	public Task Created(ModerationReport report, ProfileId author, IEnumerable<Claim> claims);
	public Task Assigned(ModerationReport report, Guid moderator, IEnumerable<Claim> claims);
	public Task Closed(ModerationReport report, Guid moderator, IEnumerable<Claim> claims);
	public Task Reopened(ModerationReport report, Guid moderator, IEnumerable<Claim> claims);
}