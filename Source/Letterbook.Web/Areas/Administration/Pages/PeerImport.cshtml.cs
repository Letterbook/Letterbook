using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class PeerImport : PageModel
{
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;
	[BindProperty] public IFormFile CsvFile { get; set; } = default!;
	[BindProperty] public bool LetterbookFormat { get; set; }
	[BindProperty] public ModerationService.MergeStrategy Strategy { get; set; }

	public PeerImport(IModerationService moderation, IAuthorizationService authz)
	{
		_moderation = moderation;
		_authz = authz;
	}

	public IActionResult OnGet()
	{
		if (!_authz.Create<Peer>(User.Claims))
			return Forbid();
		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (!_authz.Create<Peer>(User.Claims))
			return Forbid();

		await using var stream = CsvFile.OpenReadStream();
		using var reader = new StreamReader(stream);
		var csv = await reader.ReadToEndAsync();


		var peers = LetterbookFormat
			? Models.Peer.ParseLetterbook(csv)
			: Models.Peer.ParseMastodon(csv);

		if (peers is null) return BadRequest();
		await _moderation.As(User.Claims).ImportPeerRestrictions(peers, Strategy);

		return RedirectToPage(nameof(Peers));
	}

	public string FormatStrategy(string name)
	{
		return string.Join(' ', StringFormatters.SplitExp().Split(name));
	}
}