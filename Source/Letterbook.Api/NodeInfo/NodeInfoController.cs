using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.NodeInfo;

[ApiExplorerSettings(IgnoreApi = true)]
public class NodeInfoController(IOptions<CoreOptions> opts) : ControllerBase
{

	[HttpGet("/.well-known/nodeinfo")]
	public IActionResult Index()
	{
		var links = new NodeInfoLinks();
		links.Links.Add(new()
		{
			Href = opts.Value.BaseUri() + "node_info/2.1"
		});
		return Ok(links);
	}

	[HttpGet("[controller]/2.1")]
	public IActionResult NodeInfo()
	{
		var info = new NodeInfo();

		return Ok(info);
	}
}