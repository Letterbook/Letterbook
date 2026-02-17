using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Letterbook.Api.Controllers;

[Route("lb/v1")]
[Authorize(Policy = Constants.ApiPolicy)]
public class SearchController(ISearchProvider searchProvider, MappingConfigProvider mappingConfig): ControllerBase
{
	[HttpGet("search_profiles")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[SwaggerOperation("Search", "Search for a profile")]
	public async Task<IActionResult> SearchProfiles([FromQuery(Name = "q")] string query)
	{
		var mapper = new Mapper(mappingConfig.Profiles);

		var result = await searchProvider.SearchProfiles(query, CancellationToken.None, new CoreOptions(), 100);

		return Ok(mapper.Map<IEnumerable<FullProfileDto>>(result));
	}
}