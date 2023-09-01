using Newtonsoft.Json;

namespace Letterbook.Api.Tests.Support;

public class JsonFormat
{
    public static T? Parse<T>(string jsonText) => JsonConvert.DeserializeObject<T>(jsonText);
    public static string OneLine(object what) => JsonConvert.SerializeObject(what, Formatting.None);
    public static string Pretty(object what) => JsonConvert.SerializeObject(what, Formatting.Indented);
}