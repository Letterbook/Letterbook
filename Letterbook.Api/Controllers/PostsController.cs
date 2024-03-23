using System.Net;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Letterbook.Api.Controllers;

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
    public async Task<IActionResult> Post(Uuid7 profileId, [FromBody]PostDto dto, [FromQuery]bool draft = false)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    dto.Id = Uuid7.NewUuid7();
        if (_mapper.Map<Post>(dto) is not { } post)
            return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));

        var decision = _authz.Create(User.Claims, post, profileId);
        if (!decision.Allowed)
            return Unauthorized(decision);
        var pubDecision = _authz.Publish(User.Claims, post, profileId);
        if (!draft && !pubDecision.Allowed)
	        return Unauthorized(pubDecision);

        var result = await _post.As(profileId, User.Claims).Draft(post, post.InReplyTo?.GetId(), !draft);
        return Ok(_mapper.Map<PostDto>(result));
    }

    [HttpPost("{profileId}/post/{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Publish", "Publish an existing draft post")]
    public async Task<IActionResult> Publish(Uuid7 profileId, Uuid7 postId)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    if (await _post.As(profileId, User.Claims).LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Publish(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

		var result = await _post.As(profileId, User.Claims).Publish(postId);
		return Ok(_mapper.Map<PostDto>(result));
    }

    [HttpPut("{profileId}/post/{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Update", "Update an entire post, including all contents")]
    public async Task<IActionResult> Update(Uuid7 profileId, Uuid7 postId, [FromBody]PostDto dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    if (_mapper.Map<Post>(dto) is not { } post)
		    return BadRequest();

	    var decision = _authz.Publish(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

		var result = await _post.As(profileId, User.Claims).Update(postId, post);
		return Ok(_mapper.Map<PostDto>(result));
    }

    [HttpPost("{profileId}/post/{postId}/content")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Attach", "Attach additional content to a post")]
    public async Task<IActionResult> Attach(Uuid7 profileId, Uuid7 postId, [FromBody]ContentDto dto)
    {
	    var svc = _post.As(profileId, User.Claims);
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    dto.Id = Uuid7.NewUuid7();
	    if (_mapper.Map<Content>(dto) is not { } content)
		    return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));
	    if (await svc.LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Update(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

		var result = await svc.AddContent(postId, content);
		return Ok(_mapper.Map<PostDto>(result));
    }

    [HttpPut("{profileId}/post/{postId}/content/{contentId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Edit", "Edit the content of a post")]
    public async Task<IActionResult> Edit(Uuid7 profileId, Uuid7 postId, [FromBody]ContentDto dto)
    {
	    var svc = _post.As(profileId, User.Claims);
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    if (_mapper.Map<Content>(dto) is not { } content)
		    return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));
	    if (await svc.LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Update(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

		var result = await svc.UpdateContent(postId, content);
		return Ok(_mapper.Map<PostDto>(result));
    }

    [HttpDelete("{profileId}/post/{postId}/content/{contentId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Remove", "Remove content from a post")]
    public async Task<IActionResult> Remove(Uuid7 profileId, Uuid7 postId)
    {
	    throw new NotImplementedException();
    }

    [HttpDelete("{profileId}/post/{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Delete", "Delete a post")]
    public async Task<IActionResult> Delete(Uuid7 profileId, Uuid7 postId)
    {
	    throw new NotImplementedException();
    }

    [HttpGet("{profileId}/post/{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Get", "Get a post")]
    public async Task<IActionResult> Get(Uuid7 profileId, Uuid7 postId, [FromQuery]bool withThread = false)
    {
	    throw CoreException.InvalidRequest("test", "testKey", new {});
    }

    [HttpGet("{profileId}/post/drafts")]
    [ProducesResponseType<IEnumerable<PostDto>>(StatusCodes.Status200OK)]
    [SwaggerOperation("Drafts", "Get unpublished drafts")]
    public async Task<IActionResult> GetDrafts(Uuid7 profileId, Uuid7 postId, [FromQuery]bool withThread = false)
    {
	    throw CoreException.InvalidRequest("test", "testKey", new {});
    }

    [HttpGet("{profileId}/post/{postId}/replies")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Replies", "Get replies to a post")]
    public async Task<IActionResult> GetReplies(Uuid7 profileId, Uuid7 postId, [FromQuery]bool withThread = false)
    {
	    throw new NotImplementedException();
    }

    [HttpGet("{profileId}/thread/{threadId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Edit", "Edit the content of a post")]
    public async Task<IActionResult> GetThread(Uuid7 profileId, Uuid7 threadId)
    {
	    throw new NotImplementedException();
    }
}