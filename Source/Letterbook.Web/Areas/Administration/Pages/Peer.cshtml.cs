using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Peer : PageModel
{
	private readonly ILogger<Peer> _logger;
	private readonly IModerationService _moderationService;

	[FromRoute] public string Domain { get; set; } = default!;
	[BindProperty] public FormModel Form { get; set; } = default!;
	public Models.Peer Data { get; set; } = default!;

	public string PublicRemarks { get; set; } = "";
	public string PrivateNotes { get; set; } = "";

	public Peer(ILogger<Peer> logger, IModerationService moderationService)
	{
		_logger = logger;
		_moderationService = moderationService;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!Uri.TryCreate($"https://{Domain}", UriKind.Absolute, out var uri))
			return BadRequest();

		if (await _moderationService.As(User.Claims).LookupPeer(uri) is not { } data)
			return NotFound();
		Data = data;
		Form = new FormModel()
		{
			PrivateNotes = Data.PrivateComment ?? "",
			PublicRemarks = Data.PublicRemark ?? "",
			Restrictions = Enum.GetValues<Models.Restrictions>().Select((key) => new RestrictionModel()
			{
				Enabled = Data.Restrictions.ContainsKey(key),
				Expires = Data.Restrictions.TryGetValue(key, out var expires) ? expires : DateTimeOffset.MaxValue,
				Restriction = key
			}).ToList()
		};
		PublicRemarks = Data.PublicRemark ?? "";
		PrivateNotes = Data.PrivateComment ?? "";

		return Page();
	}

	public void OnPost()
	{
		_logger.LogInformation("Form Restrictions: {@Restrictions}", Form);
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
	}
}