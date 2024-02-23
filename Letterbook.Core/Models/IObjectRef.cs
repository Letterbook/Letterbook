using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// IObjectRef is the set of identifiers that we need to locate anything that could be an ActivityPub Object
/// Id is enough on its own to locate records. The other fields help to distinguish if the record is one we manage
/// locally, or if it's a cached view of a federated remote object.
/// See also IContentRef
/// </summary>
public interface IObjectRef
{
    public Uri FediId { get; set; }
    public Guid Id { get; set; }
    public string Authority { get; }

    public Uuid7 GetId();
    public string GetId25();
}