using System.Text.Json;
using System.Text.Json.Serialization;

namespace Letterbook.Api.Json;

public static class JsonOptions
{
	public static JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
	{
		Converters = { new Uuid7JsonConverter() },
	};
}