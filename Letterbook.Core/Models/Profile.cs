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
    private Profile()
    {
        Id = default!;
        Type = default;
        Host = default!;
    }
    
    public Uri Id { get; set; }
    public ActivityActorType Type { get; set; }
    public Uri Host { get; set; }
    public string? LocalId { get; set; }
    public Uri Authority { get; set; }
    public HashSet<Audience> Audiences { get; set; } = new();
}