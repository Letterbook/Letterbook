using Letterbook.Core.Models.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Letterbook.Web.Forms;

public class PostEditorForm
{
	public PostRequestDto Data { get; set; } = new();
	public List<SelectListItem> AudienceItems { get; set; } = [];

	public PostEditorForm()
	{
		Data.Contents = [];
	}
}