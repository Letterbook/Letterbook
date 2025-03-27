using Letterbook.Core;
using Medo;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class IndexModel : PageModel
{
	private ITimelineService _timeline;
	private readonly IProfileService _profiles;
	private readonly ILogger<IndexModel> _logger;

	public Models.Profile Self { get; set; } = default!;
	public IAuthzTimelineService Timeline { get; set; } = default!;
	public IAuthzProfileService Profiles { get; set; } = default!;
	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == ApplicationClaims.ActiveProfile)?.Value;

	public IndexModel(ILogger<IndexModel> logger, ITimelineService timeline, IProfileService profiles)
	{
		_timeline = timeline;
		_profiles = profiles;
		_logger = logger;
	}

	public async Task OnGet()
	{
		Timeline = _timeline.As(User.Claims);
		Profiles = _profiles.As(User.Claims);
		if (SelfId is { } id && await Profiles.LookupProfile(Models.ProfileId.FromString(id)) is { } self)
		{
			Self = self;
		}
	}

	public Task<IEnumerable<Models.Post>> GetPosts() => Timeline.GetFeed(Uuid7.Empty, DateTimeOffset.UtcNow);
}