using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class DefaultObjectCollectionResolver :
    ITypeConverter<ObjectCollection<FollowerRelation>, AsAp.IResolvable>, 
    ITypeConverter<ObjectCollection<FollowerRelation>, AsAp.Collection>
{
    public AsAp.IResolvable Convert(ObjectCollection<FollowerRelation> source, AsAp.IResolvable destination, ResolutionContext context)
    {
        return new AsAp.Link(source.Id.ToString());
    }

    public AsAp.Collection Convert(ObjectCollection<FollowerRelation> source, AsAp.Collection destination, ResolutionContext context)
    {
        return source == null ? default : new AsAp.Collection(){Id = CompactIri.FromUri(source.Id)};
    }
}