namespace Letterbook.Adapter.Db.Entities;

public class JoinObjectActor
{
    private JoinObjectActor()
    {
        ObjectId = null!;
        ActorId = null!;
        Relationship = AddressedRelationship.None;
    }
    
    public Uri ObjectId { get; set; }
    public Uri ActorId { get; set; }
    public AddressedRelationship Relationship { get; set; }
}