using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Peer : PageModel
{
	private readonly ILogger<Peer> _logger;
	private readonly IModerationService _moderationService;
	private readonly IAuthorizationService _authz;

	[FromRoute] public string Domain { get; set; } = default!;
	[BindProperty] public FormModel Form { get; set; } = default!;
	public Models.Peer Data { get; set; } = default!;

	public string PublicRemarks { get; set; } = "";
	public string PrivateNotes { get; set; } = "";

	public Peer(ILogger<Peer> logger, IModerationService moderationService, IAuthorizationService authz)
	{
		_logger = logger;
		_moderationService = moderationService;
		_authz = authz;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!_authz.Update<Models.Peer>(User.Claims))
			return Forbid();
		if (!Uri.TryCreate($"https://{Domain}", UriKind.Absolute, out var uri))
			return BadRequest();

		if (await _moderationService.As(User.Claims).LookupPeer(uri) is not { } data)
			return NotFound();
		Data = data;
		Form = FormModel.FromPeer(Data);
		PublicRemarks = Data.PublicRemark ?? "";
		PrivateNotes = Data.PrivateComment ?? "";

		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (!_authz.Update<Models.Peer>(User.Claims))
			return Forbid();
		if (!Uri.TryCreate($"https://{Domain}", UriKind.Absolute, out var uri))
			return BadRequest();

		var peer = await _moderationService.As(User.Claims).GetOrInitPeer(uri);
		peer.PublicRemark = Form.PublicRemarks;
		peer.PrivateComment = Form.PrivateNotes;
		peer.Restrictions = Form.Restrictions.Where(e => e.Enabled).ToDictionary(e => e.Restriction, e => e.Expires.ToUniversalTime());

		Data = await _moderationService.As(User.Claims).UpdatePeer(peer);
		Form = FormModel.FromPeer(Data);
		PublicRemarks = Data.PublicRemark ?? "";
		PrivateNotes = Data.PrivateComment ?? "";

		return Page();
	}

	public string FormatRestriction(string name)
	{
		return string.Join(' ', StringFormatters.SplitExp().Split(name));
	}

	public class RestrictionModel
	{
		public Models.Restrictions Restriction { get; set; }
		public DateTimeOffset Expires { get; set; }
		public bool Enabled { get; set; }
	}

	public class FormModel
	{
		public string PublicRemarks { get; set; } = "";
		public string PrivateNotes { get; set; } = "";
		public List<RestrictionModel> Restrictions { get; set; } = [];

		public static FormModel FromPeer(Models.Peer peer)
		{
			return new FormModel()
			{
				PrivateNotes = peer.PrivateComment ?? "",
				PublicRemarks = peer.PublicRemark ?? "",
				Restrictions = Enum.GetValues<Models.Restrictions>().Select((key) => new RestrictionModel()
				{
					Expires = peer.Restrictions.TryGetValue(key, out var expires) && !expires.Expired() ? expires : DateTimeOffset.MaxValue,
					Enabled = peer.Restrictions.TryGetValue(key, out var enabled) && !enabled.Expired(),
					Restriction = key
				}).ToList()
			};
		}
	}
}