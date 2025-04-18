using Letterbook.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.ActivityPub;

/// <summary>
/// Provides the Collection endpoints specified for all objects in the ActivityPub spec
/// https://www.w3.org/TR/activitypub/#collections
/// </summary>
[ApiExplorerSettings(GroupName = Docs.ActivityPubV1)]
[ApiController]
[Route("[controller]/{type}")]
[Authorize(policy: "ActivityPub")]
public class ObjectController
{
	[HttpGet]
	[Route("{id}/collections/[action]")]
	public IActionResult Likes(int id, string type)
	{
		throw new NotImplementedException();
	}

	[HttpGet]
	[Route("{id}/collections/[action]")]
	public IActionResult Shares(int id, string type)
	{
		throw new NotImplementedException();
	}
}