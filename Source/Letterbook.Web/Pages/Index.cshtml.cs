using Letterbook.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Pages;

public class IndexModel : PageModel
{
	private ITimelineService _timeline;
	private readonly IProfileService _profiles;
	private readonly IPostService _posts;
	private readonly ILogger<IndexModel> _logger;

	public IOptions<CoreOptions> Opts { get; }
	public Models.Profile Self { get; set; } = default!;
	public IAuthzTimelineService Timeline { get; set; } = default!;
	public IAuthzProfileService Profiles { get; set; } = default!;
	public IAuthzPostService Posts { get; set; } = default!;
	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == ApplicationClaims.ActiveProfile)?.Value;

	public IndexModel(ILogger<IndexModel> logger, IOptions<CoreOptions> opts, ITimelineService timeline, IProfileService profiles, IPostService posts)
	{
		Opts = opts;
		_timeline = timeline;
		_profiles = profiles;
		_posts = posts;
		_logger = logger;
	}

	public async Task OnGet()
	{
		Timeline = _timeline.As(User.Claims);
		Profiles = _profiles.As(User.Claims);
		Posts = _posts.As(User.Claims);
		if (Models.ProfileId.TryParse(SelfId, out var id) && await Profiles.LookupProfile(id) is {} self)
		{
			Self = self;
		}
	}

	// TODO: figure out what to do for anonymous feed/homepage?
	public Task<IEnumerable<Models.Post>> GetPosts() => Timeline.GetFeed(Self.Id, DateTimeOffset.UtcNow);
}