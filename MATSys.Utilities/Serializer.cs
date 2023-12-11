
namespace MATSys.Utilities;

public class Serializer
{

    public static string Serialize(object input, bool indented)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        var opt = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return System.Text.Json.JsonSerializer.Serialize(input, opt);
#elif NET35
        var setting = new Newtonsoft.Json.JsonSerializerSettings();
        if (indented)
        {
            setting.Formatting = Newtonsoft.Json.Formatting.Indented;
            return Newtonsoft.Json.JsonConvert.SerializeObject(input, setting);
        }
        else
        {
            setting.Formatting = Newtonsoft.Json.Formatting.None;
            return Newtonsoft.Json.JsonConvert.SerializeObject(input, setting);
        }
#endif
    }
    public static string Serialize(object input, Type t, bool indented)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        var opt = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return System.Text.Json.JsonSerializer.Serialize(input, t, opt);
#elif NET35
        var setting = new Newtonsoft.Json.JsonSerializerSettings();
        if (indented)
        {
            setting.Formatting = Newtonsoft.Json.Formatting.Indented;
            return Newtonsoft.Json.JsonConvert.SerializeObject(input, setting);
        }
        else
        {
            setting.Formatting = Newtonsoft.Json.Formatting.None;
            return Newtonsoft.Json.JsonConvert.SerializeObject(input, setting);
        }
#endif
    }
    public static object Deserialize(string rawString, Type t)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        return System.Text.Json.JsonSerializer.Deserialize(rawString, t)!;
#elif NET35
        return Newtonsoft.Json.JsonConvert.DeserializeObject(rawString, t)!;
#endif
    }

}
