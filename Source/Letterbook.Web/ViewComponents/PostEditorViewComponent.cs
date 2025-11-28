using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Web.ViewComponents;

public class PostEditorViewComponent : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync()
	{
		await Task.CompletedTask;
		return View(default(Models.Profile));
	}
}