using AutoMapper;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class DefaultObjectCollectionResolver :
    IMemberValueResolver<AsAp.Actor, Models.Profile, AsAp.IResolvable, ObjectCollection<FollowerRelation>>
{
    public ObjectCollection<FollowerRelation> Resolve(AsAp.Actor source, Models.Profile destination,
        AsAp.IResolvable sourceMember, ObjectCollection<FollowerRelation> destMember,
        ResolutionContext context)
    {
        destMember ??= new ObjectCollection<FollowerRelation>();
        destMember.Id = sourceMember.Id ?? destMember.Id;
        return destMember;
    }
}