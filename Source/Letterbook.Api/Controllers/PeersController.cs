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

	// [HttpPost("import")]
	// [ProducesResponseType(StatusCodes.Status200OK)]
	// [SwaggerOperation("Import", "Import a list of restricted peers")]
	// public async Task<IActionResult> ImportDenyList([FromForm] string csv, [FromQuery] DenyListFormat format,
	// 	[FromQuery] ModerationService.MergeStrategy mergeStrategy)
	// {
	// 	if (!_authz.Create<Peer>(User.Claims))
	// 		return Forbid();
	//
	// 	var peers = format switch
	// 	{
	// 		DenyListFormat.Letterbook => ParseLetterbook(csv),
	// 		DenyListFormat.Mastodon => ParseMastodon(csv),
	// 		_ => null
	// 	};
	// 	if (peers is null) return BadRequest();
	// 	await _moderation.As(User.Claims).ImportPeerRestrictions(peers, mergeStrategy);
	//
	// 	return Ok();
	// }

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
				DenyListFormat.Letterbook => ParseLetterbook(csv),
				DenyListFormat.Mastodon => ParseMastodon(csv),
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

	private static List<Peer> ParseLetterbook(string csv)
	{
		throw new NotImplementedException();
		// var now = DateTimeOffset.UtcNow;
		// using var reader = Sep.Reader().FromText(csv);
		// foreach (var required in Enum.GetValues<Restrictions>().Except([Restrictions.None]))
		// {
		// 	if (!reader.Header.ColNames.Contains(required.ToString()))
		// 		throw CoreException.InvalidRequest($"Missing required header {required}");
		// }
	}

	private static List<Peer> ParseMastodon(string csv)
	{
		var now = DateTimeOffset.UtcNow;
		using var reader = Sep.Reader().FromText(csv);
		foreach (var required in MastodonHeaders.All)
		{
			if (!reader.Header.ColNames.Contains(required))
				throw CoreException.InvalidRequest($"Missing required header {required}");
		}

		var peers = new List<Peer>();
		foreach (var row in reader)
		{
			var peer = new Peer(row[MastodonHeaders.Domain].ToString());
			switch (row[MastodonHeaders.Severity].ToString())
			{
				case "silence":
					peer.Restrictions.Add(Restrictions.LimitDiscovery, DateTimeOffset.MaxValue);
					peer.Restrictions.Add(Restrictions.Warn, DateTimeOffset.MaxValue);
					break;
				case "suspend":
				default:
					peer.Restrictions.Add(Restrictions.Defederate, DateTimeOffset.MaxValue);
					break;
			}

			if (row[MastodonHeaders.RejectMedia].TryParse<bool>(out var rejectMedia) && rejectMedia)
			{
				peer.Restrictions.Add(Restrictions.DenyAttachments, DateTimeOffset.MaxValue);
			}

			if (row[MastodonHeaders.RejectReports].TryParse<bool>(out var rejectReports) && rejectReports)
			{
				peer.Restrictions.Add(Restrictions.DenyReports, DateTimeOffset.MaxValue);
			}

			if (row[MastodonHeaders.Obfuscate].TryParse<bool>(out var obfuscate) && !obfuscate)
				peer.PublicRemark = row[MastodonHeaders.PublicComment].ToString();

			peer.PrivateComment = $"Imported at {now}";
			peers.Add(peer);
		}

		return peers;
	}
}

public static class MastodonHeaders
{
	public static string[] All = [Domain, Severity, RejectMedia, RejectReports, PublicComment, Obfuscate];

	public static string Domain => "#domain";
	public static string Severity => "#severity";
	public static string RejectMedia => "#reject_media";
	public static string RejectReports => "#reject_reports";
	public static string PublicComment => "#public_comment";
	public static string Obfuscate => "#obfuscate";
}
