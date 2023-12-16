using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameObjectJsonConverter : JsonConverter<GameObject>
{
    public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value.name);
        writer.WritePropertyName("hideFlags");
        writer.WriteValue(value.hideFlags);
        writer.WritePropertyName("layer");
        writer.WriteValue(value.layer);
        writer.WritePropertyName("tag");
        writer.WriteValue(value.tag);
        writer.WritePropertyName("active");
        writer.WriteValue(value.activeSelf);
        writer.WritePropertyName("components");
        writer.WriteStartArray();
        foreach (var component in value.GetComponents<Component>())
        {
            serializer.Serialize(writer, component);
        }
        writer.WriteEndObject();
    }

    public override GameObject ReadJson(JsonReader reader, System.Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var gameObject = new GameObject(obj["name"].Value<string>());
        gameObject.tag = obj["tag"].Value<string>();
        gameObject.layer = obj["layer"].Value<int>();
        gameObject.SetActive(obj["activeSelf"].Value<bool>());
        gameObject.isStatic = obj["isStatic"].Value<bool>();
        gameObject.hideFlags = obj["hideFlags"].Value<HideFlags>();
        gameObject.transform.position = obj["transform"]["position"].ToObject<Vector3>();
        gameObject.transform.rotation = obj["transform"]["rotation"].ToObject<Quaternion>();
        gameObject.transform.localScale = obj["transform"]["localScale"].ToObject<Vector3>();
        return gameObject;
    }
}