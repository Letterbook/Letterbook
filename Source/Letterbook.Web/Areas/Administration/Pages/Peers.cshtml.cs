using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Peers : PageModel
{
	private readonly ILogger<Peers> _logger;
	private IModerationService _moderationService;
	private IAuthorizationService _authz;
	private int _limit = 20;

	[FromQuery]
	public string? Search { get; set; }

	[FromQuery(Name = "cursor")]
	public string? ListCursor { get; set; }

	[FromQuery]
	public int Limit
	{
		get => _limit;
		set => _limit = Math.Clamp(value, 20, 100);
	}

	// public IAuthzModerationService ModerationService { get; set; } = default!;

	public List<Models.Peer> PeerList { get; set; } = [];

	public Peers(ILogger<Peers> logger, IModerationService moderationService, IAuthorizationService authz)
	{
		_logger = logger;
		_moderationService = moderationService;
		_authz = authz;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!_authz.List<Models.Peer>(User.Claims))
			return Forbid();

		return Search is not null ? await GetSearch() : await GetList();
	}

	private async Task<IActionResult> GetList()
	{
		IAsyncEnumerable<Models.Peer> results;
		if (ListCursor is not null &&
		    Uri.TryCreate($"https://{ListCursor}", UriKind.Absolute, out var uri))
		{
			results = _moderationService.As(User.Claims).ListPeers(uri, Limit);
		}
		else
		{
			results = _moderationService.As(User.Claims).ListPeers();
		}
		await foreach (var peer in results)
		{
			PeerList.Add(peer);
		}

		return Page();
	}

	private async Task<IActionResult> GetSearch()
	{
		if (!Uri.TryCreate(Search, UriKind.Absolute, out var uri) &&
		    !Uri.TryCreate($"https://{Search}", UriKind.Absolute, out uri))
			return BadRequest($"{Search} is not a valid domain name");

		var results = _moderationService.As(User.Claims).SearchPeers(uri.GetAuthority());
		await foreach (var peer in results)
		{
			PeerList.Add(peer);
		}

		return Page();
	}
}