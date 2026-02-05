using System.Text.Json;
using System.Text.Json.Serialization;

namespace Letterbook.Core.Models.Mappers.Converters;

public class ExceptionConverter : JsonConverter<Exception>
{
	public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}