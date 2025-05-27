using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Thread : PageModel
{
	private readonly ILogger<Thread> _logger;
	private readonly IProfileService _profiles;
	private readonly IPostService _posts;
	private Models.Profile _self = default!;
	private readonly Mapper _mapper;

	public IAuthzPostService Posts { get; set; } = default!;
	public IAuthzProfileService Profiles { get; set; } = default!;
	public Models.Post Post { get; set; } = default!;

	public Models.ThreadContext Context => Post.Thread;
	public IEnumerable<Models.Post> Ancestors => GetAncestors();

	public string? SelfId => User.Claims.FirstOrDefault(c => c.Type == "activeProfile")?.Value;

	public Thread(ILogger<Thread> logger, IProfileService profiles, IPostService posts, MappingConfigProvider maps)
	{
		_logger = logger;
		_profiles = profiles;
		_posts = posts;
		_mapper = new Mapper(maps.Posts);
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

	public async Task<IActionResult> OnPost(string postId, [FromForm]PostRequestDto form)
	{
		if (!Models.PostId.TryParse(postId, out var id)) return BadRequest();
		Profiles = _profiles.As(User.Claims);
		Posts = _posts.As(User.Claims);
		var selfId = await GetUserProfile();
		if (selfId == default) return Unauthorized();

		if (await Posts.LookupPost(id, true) is not { } parent)
			return NotFound();

		_logger.LogDebug("{@Form}", form);

		var post = _mapper.Map<Models.Post>(form);
		await Posts.Draft(selfId, post, parent.Id, true);

		return RedirectToPage(GetType().Name, new { postId = parent.Id });
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