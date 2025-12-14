using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Web.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Web.ViewComponents;

public class PostEditorViewComponent : ViewComponent
{
	private readonly ILogger<PostEditorViewComponent> _logger;
	private readonly IProfileService _profiles;

	public PostEditorViewComponent(ILogger<PostEditorViewComponent> logger, IProfileService profiles)
	{
		_logger = logger;
		_profiles = profiles;
	}

	public async Task<IViewComponentResult> InvokeAsync(Models.ProfileId? selfId, Models.PostId? postId)
	{
		_logger.LogDebug("Loading PostEditor");
		var model = new PostEditorForm();
		if(selfId is not null &&
		   await _profiles.As((User.Identity as ClaimsIdentity)?.Claims ?? []).LookupProfile(selfId.Value) is { } profile)
		{
			model.Audience = profile.Headlining
				.Except([Models.Audience.FromMention(profile)])
				.Prepend(Models.Audience.Public)
				.ToList();
		}

		return View(model);
	}
}