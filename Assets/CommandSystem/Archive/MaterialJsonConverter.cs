using System;
using Newtonsoft.Json;
using UnityEngine;

namespace CommandSystem.Json
{
    public class MaterialJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Material);
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var t = serializer.Deserialize(reader);
            var iv = JsonConvert.DeserializeObject<Material>(t.ToString());
            return iv;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var m = (Material)value;
            
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(m.name);
            writer.WritePropertyName("shader");
            writer.WriteValue(m.shader.name);
            writer.WritePropertyName("renderQueue");
            writer.WriteValue(m.renderQueue);
            writer.WritePropertyName("color");
            writer.WriteValue(JsonConvert.SerializeObject(m.color, new ColorJsonConverter()));
            writer.WritePropertyName("mainTexture");
            writer.WriteValue(m.mainTexture);
            writer.WritePropertyName("mainTextureOffset");
            writer.WriteValue(JsonConvert.SerializeObject(m.mainTextureOffset, new Vector2JsonConverter()));
            writer.WritePropertyName("mainTextureScale");
            writer.WriteValue(JsonConvert.SerializeObject(m.mainTextureScale, new Vector2JsonConverter()));
            writer.WritePropertyName("renderQueue");
            writer.WriteValue(m.renderQueue);
            writer.WritePropertyName("shaderKeywords");
            writer.WriteStartArray();
            foreach (var keyword in m.shaderKeywords)
            {
                writer.WriteValue(keyword);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("globalIlluminationFlags");
            writer.WriteValue(m.globalIlluminationFlags);
            writer.WritePropertyName("enableInstancing");
            writer.WriteValue(m.enableInstancing);
            writer.WritePropertyName("doubleSidedGI");
            writer.WriteValue(m.doubleSidedGI);
            writer.WritePropertyName("hideFlags");
            writer.WriteValue(m.hideFlags);
            writer.WriteEndObject();
        }
    }
}