namespace Letterbook.Core.Models.WebFinger;

/*
    {
      "subject": "acct:Gargron@mastodon.social",
      "aliases": [
        "https://mastodon.social/@Gargron",
        "https://mastodon.social/users/Gargron"
      ],
      "links": [
        {
          "rel": "http://webfinger.net/rel/profile-page",
          "type": "text/html",
          "href": "https://mastodon.social/@Gargron"
        },
        {
          "rel": "self",
          "type": "application/activity+json",
          "href": "https://mastodon.social/users/Gargron"
        },
        {
          "rel": "http://ostatus.org/schema/1.0/subscribe",
          "template": "https://mastodon.social/authorize_interaction?uri={uri}"
        }
      ]
    } 
 */
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