using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Adapter.TimescaleFeeds.EntityModels;

public class TimelineProfile
{

	public required Guid Id { get; set; }
	public required Uri FediId { get; set; }
	public required string DisplayName { get; set; }
	public required string Authority { get; set; }

	public static explicit operator TimelineProfile(Profile p) => new TimelineProfile
	{
		Id = (Uuid7)p.Id,
		FediId = p.FediId,
		DisplayName = p.DisplayName,
		Authority = p.Authority
	};

	public static explicit operator Profile(TimelineProfile t)
	{
		var p = Profile.CreateEmpty(Uuid7.FromGuid(t.Id), t.FediId);
		p.DisplayName = t.DisplayName;

		return p;
	}
}