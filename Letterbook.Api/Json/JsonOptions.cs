using System.Text.Json;

namespace Letterbook.Api.Json;

public static class JsonOptions
{
	public static JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
	{
		Converters = { new Uuid7JsonConverter() }
	};
}