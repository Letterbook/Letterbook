namespace Letterbook.Api;

// https://datatracker.ietf.org/doc/html/rfc7033#section-4
public class WebFingerJsonResourceDescriptor
{
    public string Subject { get; set; }
    public List<string> Aliases { get; set; } = new();
    public List<string> Properties { get; set; } = new();
    public List<Link> Links { get; set; } = new();
}

public class Link
{
    public string Rel { get; set; }
    public string Type { get; set; }
    public string Href { get; set; }
    public string Template { get; set; }
}