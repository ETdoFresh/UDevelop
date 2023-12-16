using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector3JsonConverter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WritePropertyName("z");
        writer.WriteValue(value.z);
        writer.WriteEndObject();
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        return new Vector3(obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>());
    }
}