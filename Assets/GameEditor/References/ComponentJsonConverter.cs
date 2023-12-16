using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ComponentJsonConverter : ComponentJsonConverter<Component> { }

public class ComponentJsonConverter<T> : JsonConverter<T> where T : Component
{
    protected static void WriteType(JsonWriter writer, Component value)
    {
        writer.WritePropertyName("$type");
        writer.WriteValue($"{value.GetType().FullName}, {value.GetType().Assembly.GetName().Name}");
    }
    
    protected static void WriteMembers(JsonWriter writer, Component value, JsonSerializer serializer, bool skipDefaultValues = false)
    {
        var members = value.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var fakeInstance = skipDefaultValues ? Activator.CreateInstance(value.GetType()) as Component : null;
        foreach (var member in members)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                var field = (FieldInfo)member;
                if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                {
                    var fieldType = field.FieldType;
                    if (typeof(Object).IsAssignableFrom(fieldType))
                    {
                        var fieldName = field.Name;
                        var fieldValue = "TODO: Handle Object fields";
                        writer.WritePropertyName(fieldName);
                        serializer.Serialize(writer, fieldValue);
                    }
                    else
                    {
                        var fieldName = field.Name;
                        var fieldValue = field.GetValue(value);
                        if (skipDefaultValues)
                        {
                            var defaultValue = field.GetValue(fakeInstance);
                            if (fieldValue.Equals(defaultValue)) continue;
                        }
                        writer.WritePropertyName(fieldName);
                        serializer.Serialize(writer, fieldValue);
                    }
                }
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                var property = (PropertyInfo)member;
                if (property.CanRead && property.CanWrite && property.GetCustomAttribute<SerializeField>() != null)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(value);
                    if (skipDefaultValues)
                    {
                        var defaultValue = property.GetValue(fakeInstance);
                        if (propertyValue.Equals(defaultValue)) continue;
                    }
                    writer.WritePropertyName(propertyName);
                    serializer.Serialize(writer, property.GetValue(value));
                }
            }
        }
        
        Object.DestroyImmediate(fakeInstance);
    }
    
    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        WriteType(writer, value);
        WriteMembers(writer, value, serializer);
        writer.WriteEndObject();
    }

    public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var gameObject = obj["gameObject"].ToObject<GameObject>();
        var component = gameObject.AddComponent(objectType);
        return (T)component;
    }
}