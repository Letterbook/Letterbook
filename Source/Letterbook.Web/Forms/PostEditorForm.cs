namespace Letterbook.Web.Forms;

public class PostEditorForm
{
	public PostEditorFormData Data { get; set; } = new();
	public List<Models.Audience> Audience { get; set; } = [];
}

public class PostEditorFormData
{
	public Models.PostId Id;
	public List<Models.ProfileId> Authors { get; set; } = [];
	public PostEditorContentData Note { get; set; } = new();
	public Dictionary<string, string> Audience { get; set; } = [];
}

public class PostEditorContentData
{
	public Guid Id;
	public string Contents { get; set; } = "";
}