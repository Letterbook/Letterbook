using Letterbook.Core;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class IndexModel : PageModel
{
	private ITimelineService _timeline;
	private readonly ILogger<IndexModel> _logger;
	private IAuthzTimelineService Timeline { get; set; }

	public Task<IEnumerable<Models.Post>> Posts => Timeline.GetFeed(Uuid7.Empty, DateTimeOffset.UtcNow);

	public IndexModel(ILogger<IndexModel> logger, ITimelineService timeline)
	{
		_timeline = timeline;
		_logger = logger;
		// _timeline = default!;
	}

	public async Task OnGet()
	{
		Timeline = _timeline.As(User.Claims);
		// posts = await _timeline.As(User.Claims).GetFeed(Uuid7.Empty, DateTimeOffset.Now);
	}
}