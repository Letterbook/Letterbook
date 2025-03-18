using System.Net.Mime;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Letterbook.Adapter.Db;

public class ContentTypeConverter() : ValueConverter<ContentType?, string?>(type => type != null ? type.ToString() : default, s => s != null ? new ContentType(s) : default);