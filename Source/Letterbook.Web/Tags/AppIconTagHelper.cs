using Letterbook.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Tags;

public class AppIconTagHelper : TagHelper
{
	public required Icon Type { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "svg";
		// output.Attributes.SetAttribute("alt", Type.ToString("G"));
		output.Attributes.Add("class", $"icon icon-{Type.ToString("G").ToLower()}");
		output.TagMode = TagMode.StartTagAndEndTag;
	}
}