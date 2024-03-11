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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PostsController(
    ILogger<PostsController> logger,
    IOptions<CoreOptions> options,
    IOptions<ApiOptions> apiOptions,
    IPostService post,
    IProfileService profile,
    IAuthorizationService authz,
    MappingConfigProvider mappingConfig)
    : ControllerBase
{
    private readonly ILogger<PostsController> _logger = logger;
    private readonly CoreOptions _options = options.Value;
    private readonly ApiOptions _apiOptions = apiOptions.Value;
    private readonly IPostService _post = post;
    private readonly IProfileService _profile = profile;
    private readonly IAuthorizationService _authz = authz;
    private readonly IMapper _mapper = new Mapper(mappingConfig.Posts);

    // [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("{profileId}/post")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Draft(string profileId, [FromBody]PostDto dto)
    {
	    await LogBody(HttpContext.Request.Body, _logger);
        if (!Id.TryAsUuid7(profileId, out var id))
            return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {nameof(profileId)}"));
        if (_mapper.Map<Post>(dto) is not { } post)
            return BadRequest(new ErrorMessage(ErrorCodes.InvalidRequest, $"Invalid {typeof(PostDto)}"));

        var decision = _authz.Create(User.Claims, post, id);
        if (!decision.Allowed)
            return Unauthorized(decision);

        try
        {
            var result = await _post.Draft(post);
            return Ok(_mapper.Map<PostDto>(result));
        }
        catch (CoreException e)
        {
            var message = new ErrorMessage(e);
            _logger.LogDebug(e, "Error {Code} while posting draft from {Id}", message.ErrorCode, id);

            if (e.Flagged(ErrorCodes.InternalError))
                return base.StatusCode((int)HttpStatusCode.InternalServerError, message);
            if (e.Flagged(ErrorCodes.PermissionDenied))
                return Unauthorized(message);
            return BadRequest(message);
        }
    }

    private async ValueTask LogBody(Stream body, ILogger logger)
    {
	    if (!logger.IsEnabled(LogLevel.Debug) || !body.CanRead) return;

	    try
	    {
		    body.Seek(0, SeekOrigin.Begin);
		    var buffer = new byte[body.Length];
		    var read = await body.ReadAsync(buffer, 0, (int)body.Length);

		    if (read > 0)
		    {
			    logger.LogDebug("Raw body {Json}", Encoding.UTF8.GetString(buffer));
		    }
		    else logger.LogDebug("Couldn't reread the request body");
	    }
	    catch (Exception e)
	    {
		    logger.LogError(e, "Error logging raw body {Error}", e.Message);
	    }
    }
    // draft
    // publish
    //
}