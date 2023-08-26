using Newtonsoft.Json;

namespace Letterbook.Core;

public class JsonFormat
{
    public static T? Parse<T>(string jsonText) => JsonConvert.DeserializeObject<T>(jsonText);
    public static string OneLine(object what) => JsonConvert.SerializeObject(what, Formatting.None);
    public static string Pretty(object what) => JsonConvert.SerializeObject(what, Formatting.Indented);
}