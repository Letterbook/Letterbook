using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Web.Forms;
using Letterbook.Web.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Web.Controllers
{
	[Route("forms/[controller]/[action]")]
    public class PostEditorController : ControllerBase
    {
	    private readonly ILogger<PostEditorController> _logger;
	    private readonly IPostService _posts;
	    private readonly IAuthorizationService _authz;
	    private readonly Mapper _mapper;

	    public PostEditorController(ILogger<PostEditorController> logger, IPostService posts, IAuthorizationService authz, FormsProfileProvider formMaps)
	    {
		    _logger = logger;
		    _posts = posts;
		    _authz = authz;
		    _mapper = new Mapper(formMaps.Profile);
	    }

        [HttpPost]
        public async Task<IActionResult> Submit([FromForm] PostEditorForm form)
        {
	        if (!ModelState.IsValid)
		        return BadRequest(ModelState);
	        if (!_authz.Create<Models.Post>(User.Claims) && !_authz.Update<Models.Post>(User.Claims))
		        return Forbid();
	        if (!User.Claims.TryGetActiveProfileId(out var selfId))
		        return Challenge();

	        var post = _mapper.Map<Models.Post>(form.Data);
	        var svc = _posts.As(User.Claims);
	        var result = await svc.Draft(selfId, post, post.InReplyTo?.GetId(), publish: true);

	        if (result is null) return BadRequest();
	        // var redirect = Url.PageLink(nameof(Pages.Thread), values: new { PostId = result.Id });

	        // return Redirect($"/thread/{result.Id}");
	        return RedirectToPage("/" + nameof(Pages.Thread), new { postId = result.Id });
        }
    }
}
