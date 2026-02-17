using Letterbook.Core;
using Letterbook.Core.Adapters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Letterbook.Api.Controllers;

[Route("lb/v1")]
public class SearchController(ISearchProvider searchProvider): ControllerBase
{
	[HttpGet("search_profiles")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[SwaggerOperation("Search", "Search for a profile")]
	public IActionResult SearchProfiles([FromQuery(Name = "q")] string query)
	{
		searchProvider.SearchProfiles(query, CancellationToken.None, new CoreOptions
		{

		}, 100);
		return Ok();
	}
}