namespace Letterbook.Api;

// https://datatracker.ietf.org/doc/html/rfc7033#section-4
public class WebFingerJsonResourceDescriptor
{
    public string Subject { get; set; }
    public List<string> Aliases { get; set; }
    public List<string> Properties { get; set; }
    public List<Link> Links { get; set; }
}

public class Link
{
    public string Rel { get; set; }
    public string Type { get; set; }
    public string Href { get; set; }
    public string Template { get; set; }
}