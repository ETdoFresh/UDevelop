using System;
using Newtonsoft.Json;
using UnityEngine;

namespace CommandSystem.Json
{
    public class ColorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = serializer.Deserialize(reader);
            var iv = JsonConvert.DeserializeObject<Color>(t.ToString());
            return iv;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color)value;
            
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(c.r);
            writer.WritePropertyName("g");
            writer.WriteValue(c.g);
            writer.WritePropertyName("b");
            writer.WriteValue(c.b);
            writer.WritePropertyName("a");
            writer.WriteValue(c.a);
            writer.WriteEndObject();
        }
    }
}