namespace Letterbook.Core.Models;

/// <summary>
/// Ok, so. The AP Spec says that Actors are Objects. But, here's the thing. Most of the object properties make no
/// sense on an actor. What would it even mean for an actor to be "addressed to" something? So, I'm treating them as
/// separate entities for now.
///
/// Anyway, I'm really thinking of AP types as DTOs, not application models
/// </summary>
public class Actor
{
    private Actor()
    {
        Id = default!;
        Type = default;
        Host = default!;
    }
    
    public Uri Id { get; set; }
    public ActivityObjectType Type { get; set; }
    public Uri Host { get; set; }
    public string? LocalId { get; set; }
    public List<Audience> Audiences { get; set; } = new();
}