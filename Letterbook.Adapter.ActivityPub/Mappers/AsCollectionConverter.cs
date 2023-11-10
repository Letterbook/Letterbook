using AutoMapper;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class AsCollectionConverter : ITypeConverter<AsAp.Collection, Models.ObjectCollection<Models.FollowerRelation>>
{
    public Models.ObjectCollection<Models.FollowerRelation> Convert(AsAp.Collection source, Models.ObjectCollection<Models.FollowerRelation> destination, ResolutionContext context)
    {
        return new Models.ObjectCollection<Models.FollowerRelation>(source.Id);
    }
}