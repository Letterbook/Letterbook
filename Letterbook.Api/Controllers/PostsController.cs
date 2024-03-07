using System.Net;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Letterbook.Core.IAuthorizationService;

namespace Letterbook.Api.Controllers;

[ApiExplorerSettings(GroupName = Docs.LetterbookV1)]
[Route("lb/v1/[controller]")]
public class PostsController(
    ILogger<PostsController> logger,
    CoreOptions options,
    ApiOptions apiOptions,
    IPostService post,
    IProfileService profile,
    IAuthorizationService authz)
    : ControllerBase
{
    private readonly ILogger<PostsController> _logger = logger;
    private readonly CoreOptions _options = options;
    private readonly ApiOptions _apiOptions = apiOptions;
    private readonly IPostService _post = post;
    private readonly IProfileService _profile = profile;
    private readonly IAuthorizationService _authz = authz;
    private readonly IMapper _mapper = new Mapper(DtoMapper.Default);

    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{ProfileId}/post")]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Draft(Id25 profileId, [FromBody]PostDto dto)
    {
        if (!profileId.TryAsUuid7(out var id))
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
    // draft
    // publish
    //
}