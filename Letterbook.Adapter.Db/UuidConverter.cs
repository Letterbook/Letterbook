using JetBrains.Annotations;
using Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Letterbook.Adapter.Db;

[UsedImplicitly]
public class UuidConverter() : ValueConverter<Uuid7, byte[]>(uuid7 => uuid7.ToByteArray(), bytes => new Uuid7(bytes), Hints)
{
    private static readonly ConverterMappingHints Hints = new(size: 16);
}