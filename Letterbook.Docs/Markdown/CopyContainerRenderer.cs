using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Letterbook.Docs.Markdown;

public class CopyContainerRenderer : HtmlObjectRenderer<CustomContainer>
{
	public string Class { get; set; } = "";
	public string BoxClass { get; set; } = "bg-gray-700";
	public string IconClass { get; set; } = "";
	public string TextClass { get; set; } = "text-lg text-white";

	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		renderer.EnsureLine();
		if (renderer.EnableHtmlForBlock)
		{
			renderer.Write(@$"<div class=""{Class} flex cursor-pointer mb-3"" onclick=""copy(this)"">
                <div class=""flex-grow {BoxClass}"">
                    <div class=""pl-4 py-1 pb-1.5 align-middle {TextClass}"">");
		}

		// We don't escape a CustomContainer
		renderer.WriteChildren(obj);
		if (renderer.EnableHtmlForBlock)
		{
			renderer.WriteLine(@$"</div>
                    </div>
                <div class=""flex"">
                    <div class=""{IconClass} text-white p-1.5 pb-0"">
                        <svg class=""copied w-6 h-6"" fill=""none"" stroke=""currentColor"" viewBox=""0 0 24 24"" xmlns=""http://www.w3.org/2000/svg""><path stroke-linecap=""round"" stroke-linejoin=""round"" stroke-width=""2"" d=""M5 13l4 4L19 7""></path></svg>
                        <svg class=""nocopy w-6 h-6"" title=""copy"" fill='none' stroke='white' viewBox='0 0 24 24' xmlns='http://www.w3.org/2000/svg'>
                            <path stroke-linecap='round' stroke-linejoin='round' stroke-width='1' d='M8 7v8a2 2 0 002 2h6M8 7V5a2 2 0 012-2h4.586a1 1 0 01.707.293l4.414 4.414a1 1 0 01.293.707V15a2 2 0 01-2 2h-2M8 7H6a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2v-2'></path>
                        </svg>
                    </div>
                </div>
            </div>");
		}
	}
}