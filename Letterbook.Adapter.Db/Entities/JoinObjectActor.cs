namespace Letterbook.Adapter.Db.Entities;

public class JoinObjectActor
{
    public Uri ObjectId { get; set; }
    public Uri ActorId { get; set; }
    public AddressedRelationship Relationship { get; set; }
}