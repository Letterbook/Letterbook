using AutoMapper;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Letterbook.Api.Controllers;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("/lb/v1/[controller]")]
public class TimelinesController : ControllerBase
{
	private ITimelineService _timelines;
	private readonly Mapper _mapper;

	public TimelinesController(ITimelineService timelines,MappingConfigProvider maps)
	{
		_timelines = timelines;
		_mapper = new Mapper(maps.Posts);
	}

	[HttpGet("{profileId}")]
	[SwaggerOperation("Get", "Get posts for the Profile's regular timeline feed")]
	[ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetTimeline(Uuid7 profileId, [FromQuery]DateTimeOffset? starting, [FromQuery]int count = 100)
	{
		var posts = await _timelines.As(User.Claims).GetFeed(profileId, starting ?? DateTimeOffset.UtcNow, count);
		return Ok(posts.Select(p => _mapper.Map<PostDto>(p)));
	}
}