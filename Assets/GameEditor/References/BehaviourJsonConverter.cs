using Newtonsoft.Json;
using UnityEngine;

public class BehaviourJsonConverter : ComponentJsonConverter<Behaviour>
{
    public override void WriteJson(JsonWriter writer, Behaviour value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        WriteType(writer, value);
        
        var ignoreDefaultValues = serializer.DefaultValueHandling == DefaultValueHandling.Ignore;
        if (!ignoreDefaultValues || value.enabled != true)
        {
            writer.WritePropertyName("enabled");
            writer.WriteValue(value.enabled);
        }
        if (!ignoreDefaultValues || value.hideFlags != HideFlags.None)
        {
            writer.WritePropertyName("hideFlags");
            writer.WriteValue(value.hideFlags);
        }
        
        //WriteMembers(writer, value, serializer, skipDefaultValues: true);
        WriteMembers(writer, value, serializer);
        writer.WriteEndObject();
    }
}