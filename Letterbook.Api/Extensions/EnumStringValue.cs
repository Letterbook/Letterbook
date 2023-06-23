using System.Reflection;
using Letterbook.Api.Attributes;

namespace Letterbook.Api.Extensions;

public static class EnumStringValue
{
    public static string GetStringValue(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attribute = fieldInfo?.GetCustomAttribute<StringValueAttribute>();
        
        return attribute?.StringValue ?? value.ToString();
    }
}