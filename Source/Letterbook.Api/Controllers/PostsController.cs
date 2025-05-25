using System.Net;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[Authorize(Policy = Constants.ApiPolicy)]
[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PostsController : ControllerBase
{
	private readonly ILogger<PostsController> _logger;
	private readonly CoreOptions _options;
	private readonly IPostService _post;
	private readonly IProfileService _profile;
	private readonly IAuthorizationService _authz;
	private readonly IMapper _mapper;

	public PostsController(ILogger<PostsController> logger,
		IOptions<CoreOptions> options,
		IPostService postSvc,
		IProfileService profile,
		IAuthorizationService authz,
		MappingConfigProvider mappingConfig)
	{
		_logger = logger;
		_options = options.Value;
		_post = postSvc;
		_profile = profile;
		_authz = authz;
		_mapper = new Mapper(mappingConfig.Posts);
	}

	// [Authorize(JwtBearerDefaults.AuthenticationScheme)]
	[HttpPost("{profileId}/post")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Post", "Draft a new post, and optionally publish it")]
	public async Task<IActionResult> Post(Uuid7 profileId, [FromBody] PostDto dto, [FromQuery] bool draft = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		dto.Id = null;

		if (_mapper.Map<Post>(dto) is not { } post)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));
		foreach (var content in post.Contents)
		{
			content.Id = Uuid7.NewGuid();
			content.SetLocalFediId(_options);
		}

		var decision = _authz.Create(User.Claims, post);
		if (!decision.Allowed)
			return Unauthorized(decision);
		var pubDecision = _authz.Publish(User.Claims, post);
		if (!draft && !pubDecision.Allowed)
			return Unauthorized(pubDecision);

		var result = await _post.As(User.Claims).Draft(profileId, post, post.InReplyTo?.GetId(), !draft);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpPost("{profileId}/post/{postId}")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Publish", "Publish an existing draft post")]
	public async Task<IActionResult> Publish(ProfileId profileId, Uuid7 postId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _post.As(User.Claims).Publish(profileId, postId);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpPut("{profileId}/post/{postId}")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Update", "Update an entire post, including all contents")]
	public async Task<IActionResult> Update(ProfileId profileId, Uuid7 postId, [FromBody] PostDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (_mapper.Map<Post>(dto) is not { } post)
			return BadRequest();

		var decision = _authz.Publish(User.Claims, post);
		if (!decision.Allowed)
			return Unauthorized(decision);

		var result = await _post.As(User.Claims).Update(profileId, postId, post);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpPost("{profileId}/post/{postId}/content")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Attach", "Attach additional content to a post")]
	public async Task<IActionResult> Attach(ProfileId profileId, Uuid7 postId, [FromBody] ContentDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		dto.Id = Uuid7.NewUuid7();
		if (_mapper.Map<Content>(dto) is not { } content)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));

		var result = await _post.As(User.Claims).AddContent(profileId, postId, content);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpPut("{profileId}/post/{postId}/content/{contentId}")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Edit", "Edit the content of a post")]
	public async Task<IActionResult> Edit(ProfileId profileId, Uuid7 postId, Uuid7 contentId, [FromBody] ContentDto dto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);
		if (_mapper.Map<Content>(dto) is not { } content)
			return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));

		var result = await _post.As(User.Claims).UpdateContent(profileId, postId, contentId, content);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpDelete("{profileId}/post/{postId}/content/{contentId}")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Remove", "Remove content from a post")]
	public async Task<IActionResult> Remove(ProfileId profileId, Uuid7 postId, Uuid7 contentId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _post.As(User.Claims).RemoveContent(profileId, postId, contentId);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpDelete("{profileId}/post/{postId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[SwaggerOperation("Delete", "Delete a post")]
	public async Task<IActionResult> Delete(ProfileId profileId, Uuid7 postId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		await _post.As(User.Claims).Delete(profileId, postId);
		return Ok();
	}

	[HttpGet("{profileId}/post/{postId}")]
	[ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
	[SwaggerOperation("Get", "Get a post")]
	public async Task<IActionResult> Get(ProfileId profileId, Uuid7 postId, [FromQuery] bool withThread = false)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _post.As(User.Claims).LookupPost(postId, withThread);
		return Ok(_mapper.Map<PostDto>(result));
	}

	[HttpGet("{profileId}/post/drafts")]
	[ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Drafts", "Get unpublished drafts")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<IActionResult> GetDrafts(Uuid7 profileId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		throw new NotImplementedException();
	}

	[HttpGet("{profileId}/post/{postId}/replies")]
	[ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Replies", "Get replies to a post")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<IActionResult> GetReplies(Uuid7 profileId, Uuid7 postId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		throw new NotImplementedException();
	}

	[HttpGet("{profileId}/thread/{threadId}")]
	[ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
	[SwaggerOperation("Thread", "Get a thread by id")]
	public async Task<IActionResult> GetThread(ProfileId profileId, ThreadId threadId)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _post.As(User.Claims).LookupThread(profileId, threadId);
		if (result is not null && result.Posts.Count != 0)
			return Ok(_mapper.Map<IEnumerable<PostDto>>(result.Posts));
		return NotFound();
	}
}