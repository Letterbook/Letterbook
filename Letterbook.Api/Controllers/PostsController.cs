using System.Net;
using System.Text;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PostsController(
    ILogger<PostsController> logger,
    IOptions<CoreOptions> options,
    IPostService postSvc,
    IProfileService profile,
    IAuthorizationService authz,
    MappingConfigProvider mappingConfig)
    : ControllerBase
{
    private readonly ILogger<PostsController> _logger = logger;
    private readonly CoreOptions _options = options.Value;
    private readonly IPostService _post = postSvc;
    private readonly IProfileService _profile = profile;
    private readonly IAuthorizationService _authz = authz;
    private readonly IMapper _mapper = new Mapper(mappingConfig.Posts);

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

        try
        {
            var result = await _post.Draft(post, post.InReplyTo?.GetId(), !draft);
            return Ok(_mapper.Map<PostDto>(result));
        }
        catch (CoreException e)
        {
            var message = new ErrorMessage(e);
            _logger.LogDebug(e, "Error {Code} while posting for profile {Profile}", message.ErrorCode, profileId);

            if (e.Flagged(ErrorCodes.InternalError))
                return base.StatusCode((int)HttpStatusCode.InternalServerError, message);
            if (e.Flagged(ErrorCodes.PermissionDenied))
                return Unauthorized(message);
            return BadRequest(message);
        }
    }

    [HttpPut("{profileId}/post/{postId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Publish", "Publish an existing draft post")]
    public async Task<IActionResult> Publish(Uuid7 profileId, Uuid7 postId)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    if (await _post.LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Publish(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

	    try
	    {
		    var result = await _post.Publish(postId);
		    return Ok(_mapper.Map<PostDto>(result));
	    }
	    catch (CoreException e)
	    {
		    var message = new ErrorMessage(e);
		    _logger.LogDebug(e, "Error {Code} while publishing {Post} for profile {Profile}", message.ErrorCode, postId, profileId);

		    if (e.Flagged(ErrorCodes.InternalError))
			    return base.StatusCode((int)HttpStatusCode.InternalServerError, message);
		    if (e.Flagged(ErrorCodes.PermissionDenied))
			    return Unauthorized(message);
		    return BadRequest(message);
	    }
    }


    [HttpPost("{profileId}/post/{postId}/content")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Attach", "Attach additional content to a post")]
    public async Task<IActionResult> Attach(Uuid7 profileId, Uuid7 postId, [FromBody]ContentDto dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    dto.Id = Uuid7.NewUuid7();
	    if (_mapper.Map<Content>(dto) is not { } content)
		    return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));
	    if (await _post.LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Update(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

	    try
	    {
		    var result = await _post.AddContent(postId, content);
		    return Ok(_mapper.Map<PostDto>(result));
	    }
	    catch (CoreException e)
	    {
		    var message = new ErrorMessage(e);
		    _logger.LogDebug(e, "Error {Code} while updating {Post} for profile {Profile}", message.ErrorCode, postId, profileId);

		    if (e.Flagged(ErrorCodes.InternalError))
			    return base.StatusCode((int)HttpStatusCode.InternalServerError, message);
		    if (e.Flagged(ErrorCodes.PermissionDenied))
			    return Unauthorized(message);
		    return BadRequest(message);
	    }
    }

    [HttpPut("{profileId}/post/{postId}/content/{contentId}")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [SwaggerOperation("Edit", "Edit the content of a post")]
    public async Task<IActionResult> Edit(Uuid7 profileId, Uuid7 postId, [FromBody]ContentDto dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);
	    if (_mapper.Map<Content>(dto) is not { } content)
		    return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));
	    if (await _post.LookupPost(postId, false) is not { } post)
		    return NotFound(new ErrorMessage(ErrorCodes.MissingData, $"{postId.ToId25String()} not found"));

	    var decision = _authz.Update(User.Claims, post, profileId);
	    if (!decision.Allowed)
		    return Unauthorized(decision);

	    try
	    {
		    var result = await _post.UpdateContent(postId, content);
		    return Ok(_mapper.Map<PostDto>(result));
	    }
	    catch (CoreException e)
	    {
		    var message = new ErrorMessage(e);
		    _logger.LogDebug(e, "Error {Code} while updating {Post} for profile {Profile}", message.ErrorCode, postId, profileId);

		    if (e.Flagged(ErrorCodes.InternalError))
			    return base.StatusCode((int)HttpStatusCode.InternalServerError, message);
		    if (e.Flagged(ErrorCodes.PermissionDenied))
			    return Unauthorized(message);
		    return BadRequest(message);
	    }
    }

    // draft
    // publish
    //
}