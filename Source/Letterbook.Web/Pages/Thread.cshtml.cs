using System.Collections;
using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Thread : PageModel
{
	private readonly IProfileService _profiles;
	private readonly IPostService _posts;
	private Models.Profile _self = default!;

	public IAuthzPostService Posts { get; set; } = default!;
	public IAuthzProfileService Profiles { get; set; } = default!;
	public Models.Post Post { get; set; } = default!;

	public Models.ThreadContext Context => Post.Thread;
	public IEnumerable<Models.Post> Ancestors => GetAncestors();

	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == "activeProfile")?.Value;

	public Thread(IProfileService profiles, IPostService posts)
	{
		_profiles = profiles;
		_posts = posts;
	}

	public async Task<IActionResult> OnGet(string postId)
	{
		if (!Models.PostId.TryParse(postId, out var id)) return BadRequest();
		Profiles = _profiles.As(User.Claims);
		Posts = _posts.As(User.Claims);
		var selfId = await GetUserProfile();

		if (await Posts.LookupPost(id, true) is not { } post)
			return NotFound();
		Post = post;

		return Page();
	}

	private IEnumerable<Models.Post> GetAncestors()
	{
		var post = Post;
		while (post.InReplyTo is {} parent)
		{
			post = parent;
			yield return parent;
		}
	}

	private async Task<Models.ProfileId> GetUserProfile()
	{
		if (SelfId is { } selfId && await Profiles.LookupProfile(Models.ProfileId.FromString(selfId)) is { } self)
		{
			_self = self;
			return self.Id;
		}

		return default;
	}
}