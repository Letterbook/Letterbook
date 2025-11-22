using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nietras.SeparatedValues;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PeersController : ControllerBase
{
	private readonly ILogger<PeersController> _logger;
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;

	public PeersController(ILogger<PeersController> logger, IModerationService moderation, IAuthorizationService authz)
	{
		_logger = logger;
		_moderation = moderation;
		_authz = authz;
	}

	[HttpPost("import")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[SwaggerOperation("Import", "Import a list of restricted peers")]
	public async Task<IActionResult> ImportDenyList(IFormFile csvFile, [FromQuery] DenyListFormat format,
		[FromQuery] ModerationService.MergeStrategy mergeStrategy)
	{
		if (!_authz.Create<Peer>(User.Claims))
			return Forbid();

		try
		{
			await using var stream = csvFile.OpenReadStream();
			using var reader = new StreamReader(stream);
			var csv = await reader.ReadToEndAsync();

			var peers = format switch
			{
				DenyListFormat.Letterbook => Peer.ParseLetterbook(csv),
				DenyListFormat.Mastodon => Peer.ParseMastodon(csv),
				_ => null
			};
			if (peers is null) return BadRequest();
			await _moderation.As(User.Claims).ImportPeerRestrictions(peers, mergeStrategy);

			return Ok();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

	}
}
