using AutoMapper;
using Letterbook.ActivityPub;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class DefaultOrderedCollectionResolver : 
    ITypeConverter<Models.ObjectCollection<Models.FollowerRelation>, AsAp.IResolvable>
{
    public AsAp.IResolvable Convert(Models.ObjectCollection<Models.FollowerRelation> source, AsAp.IResolvable destination, ResolutionContext context)
    {
        return new AsAp.Link(source.Id.ToString());
    }
}