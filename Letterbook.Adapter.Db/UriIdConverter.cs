using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Letterbook.Adapter.Db;

public class UriIdConverter : ValueConverter<Uri, string>
{
    public UriIdConverter() : base(uri => uri.ToString(), s => new Uri(s))
    {
    }
}

public class UriIdComparer : ValueComparer<Uri>
{
    public UriIdComparer() : base((u1, u2) => (u1 != null && u2 != null) && u1.ToString() == u2.ToString(),
        uri => uri.ToString().GetHashCode())
    {
    }
}