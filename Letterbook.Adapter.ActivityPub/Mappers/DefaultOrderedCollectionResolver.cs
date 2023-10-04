using AutoMapper;
using Letterbook.ActivityPub;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class DefaultOrderedCollectionResolver : IMemberValueResolver<Letterbook.Core.Models.Profile,Letterbook.ActivityPub.Models.Actor,Models.ObjectCollection<Models.FollowerRelation>,Letterbook.ActivityPub.Models.Collection?>
{
    public AsAp.Collection Resolve(Models.Profile source, AsAp.Actor destination,
        Models.ObjectCollection<Models.FollowerRelation> sourceMember, AsAp.Collection? destMember,
        ResolutionContext context)
    {
        destMember ??= new AsAp.Collection() { Type = "OrderedCollection" };
        destMember.Id = new CompactIri(sourceMember.Id.ToString());
        return destMember;
    }
}