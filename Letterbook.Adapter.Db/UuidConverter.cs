using JetBrains.Annotations;
using Medo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Letterbook.Adapter.Db;

[UsedImplicitly]
public class UuidConverter() : ValueConverter<Uuid7, Guid>(uuid7 => uuid7.ToGuid(), guid => Uuid7.FromGuid(guid));