﻿//HintName: TestIdIntJsonConverter.g.cs
// <auto-generated from Letterbook.Generators.TypedIdGenerator/>

using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Generators;


namespace Letterbook.Generators.Tests;

[TypedIdJsonConverter]
public class TestIdIntJsonConverter : JsonConverter<TestIdInt>
{
    /// <inheritdoc />
    public override TestIdInt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
   		try
   		{
   			var value = JsonSerializer.Deserialize<int>(ref reader, options);
   			return new TestIdInt(value);
   		}
   		catch (Exception e)
   		{
   			throw new JsonException("Unable to convert to TestIdInt", e);
   		}
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TestIdInt value, JsonSerializerOptions options)
    {
          JsonSerializer.Serialize(writer, value.Id, options);
    }
}