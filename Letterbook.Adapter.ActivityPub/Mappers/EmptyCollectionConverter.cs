using AutoMapper;
using Letterbook.ActivityPub;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class EmptyCollectionConverter : ITypeConverter<Uri, AsAp.Collection>
{
    public AsAp.Collection Convert(Uri source, AsAp.Collection destination, ResolutionContext context)
    {
        return new AsAp.Collection() { Id = CompactIri.FromUri(source) };
    }
}