namespace Letterbook.Api.NodeInfo;

public class NodeInfo
{
	public string Version { get; set; } = "2.1";
	public bool OpenRegistration { get; set; } = false;
	public Dictionary<string, string> Metadata { get; set; } = new();
	public List<string> Protocols { get; set; } = ["activitypub"];
	public Software Software { get; set; } = new();
}

public class Software
{
	public string Name { get; set; } = "Letterbook";
	public string Version { get; set; } = "0.0.0-0.sup";
	public string Repository { get; set; } = "https://github.com/letterbook/letterbook";
	public string Homepage { get; set; } = "https://letterbook.com";
	public Usage Usage { get; set; } = new();
	public Services Services { get; set; } = new();
}

public class Services
{
	public List<string> Inbound { get; set; } = [];
	public List<string> Outbound { get; set; } = [];
}

public class Usage
{
	public int LocalPosts { get; set; }
	public int LocalComments { get; set; }
	public Users Users { get; set; } = new();
}

public class Users
{
	public int Total { get; set; }
	public int ActiveHalfYear { get; set; }
	public int ActiveMonth { get; set; }
}

public class NodeInfoLinks
{
	public List<Link> Links { get; set; } = [];
}

public class Link
{
	public string Rel { get; set; } = "http://nodeinfo.diaspora.software/ns/schema/2.1";
	public required string Href { get; set; }
}