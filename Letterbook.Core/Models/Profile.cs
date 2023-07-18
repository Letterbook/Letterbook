namespace Letterbook.Core.Models;

/// <summary>
/// A Profile is the externally visible representation of an account on the network. In ActivityPub terms, it should map
/// 1:1 with Actors.
/// Local profiles are managed by one or more Accounts, which are the representation of a user internally to the system.
/// Remote profiles have no associated Accounts, and can only be created or modified by federated changes to the remote
/// Actor.
/// </summary>
public class Profile : IObjectRef
{
    protected Profile()
    {
        Id = default!;
        Type = default;
        Authority = default!;
    }
    
    public Uri Id { get; set; }
    public ActivityActorType Type { get; set; }
    public string? LocalId { get; set; }
    public string Authority { get; set; }
    public ICollection<Audience> Audiences { get; set; } = new HashSet<Audience>();
}