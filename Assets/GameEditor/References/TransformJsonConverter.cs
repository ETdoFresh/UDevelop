using System;
using Newtonsoft.Json;
using UnityEngine;

public class TransformJsonConverter : ComponentJsonConverter<Transform>
{
    public override void WriteJson(JsonWriter writer, Transform value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        WriteType(writer, value);
        
        var ignoreDefaultValues = serializer.DefaultValueHandling == DefaultValueHandling.Ignore;
        if (!ignoreDefaultValues || value.position != Vector3.zero)
        {
            writer.WritePropertyName("localPosition");
            serializer.Serialize(writer, value.localPosition);
        }
        if (!ignoreDefaultValues || value.rotation != Quaternion.identity)
        {
            writer.WritePropertyName("localRotation");
            serializer.Serialize(writer, value.localRotation);
        }
        if (!ignoreDefaultValues || value.localScale != Vector3.one)
        {
            writer.WritePropertyName("localScale");
            serializer.Serialize(writer, value.localScale);
        }
        writer.WriteEndObject();
    }

    public override Transform ReadJson(JsonReader reader, System.Type objectType, Transform existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}