using System.Text.Json;
using System.Text.Json.Serialization;
using Medo;

namespace Letterbook.Api.Json;

public class Uuid7JsonConverter : JsonConverter<Uuid7>
{
	/// <inheritdoc />
	public override Uuid7 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		try
		{
			return Uuid7.FromId25String(reader.GetString()!);
		}
		catch (Exception e)
		{
			throw new JsonException("Unable to convert to Uuid7", e);
		}
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, Uuid7 value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToId25String());
	}
}