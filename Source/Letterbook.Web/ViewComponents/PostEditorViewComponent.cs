using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Models.Dto;
using Letterbook.Web.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
			model.AudienceItems = profile.Headlining
				.Prepend(Models.Audience.Public)
				.Select(a => new SelectListItem(a.FediId.PathAndQuery + a.FediId.Fragment, a.FediId.ToString()))
				.ToList();
		}

		model.Data.Contents = [new ContentDto(){Type = "Note"}];
		// if(postId is not null &&
		   // )

		return View(model);
	}
}