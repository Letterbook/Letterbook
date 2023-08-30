namespace Letterbook.Core.Models.WebFinger;

// https://datatracker.ietf.org/doc/html/rfc7033#section-4.4
// https://datatracker.ietf.org/doc/html/rfc7033#section-4
public class WebFingerQueryTarget
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Domain { get; set; }

    public static WebFingerQueryTarget Parse(string resource)
    {
        // acct:coffee_nebula@uss-voyager.example
        var usernameAndDomain = resource.Split(":")[1].Split("@");
        var username = usernameAndDomain[0];
        var domain = usernameAndDomain[1];

        return new WebFingerQueryTarget()
        {
            Username = username,
            Domain = domain
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is WebFingerQueryTarget target)
            return Equals(target);

        return base.Equals(obj);
    }

    protected bool Equals(WebFingerQueryTarget other)
    {
        return Host == other.Host && Username == other.Username && Domain == other.Domain;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Host, Username, Domain);
    }
}