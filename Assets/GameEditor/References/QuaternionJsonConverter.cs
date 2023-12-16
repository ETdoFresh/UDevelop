using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuaternionJsonConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WritePropertyName("z");
        writer.WriteValue(value.z);
        writer.WritePropertyName("w");
        writer.WriteValue(value.w);
        writer.WriteEndObject();
    }

    public override Quaternion ReadJson(JsonReader reader, System.Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        return new Quaternion(obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>(), obj["w"].Value<float>());
    }
}