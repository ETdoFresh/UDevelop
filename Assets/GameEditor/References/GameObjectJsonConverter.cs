using System;
using GameEditor.References;
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

public class MeshFilterJsonConverter : JsonConverter<MeshFilter>
{
    public override void WriteJson(JsonWriter writer, MeshFilter value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("sharedMesh");
        serializer.Serialize(writer, value.sharedMesh);
        writer.WriteEndObject();
    }

    public override MeshFilter ReadJson(JsonReader reader, System.Type objectType, MeshFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var meshFilter = new GameObject().AddComponent<MeshFilter>();
        meshFilter.sharedMesh = obj["sharedMesh"].ToObject<Mesh>();
        return meshFilter;
    }
}

public static class UnityObjectReferenceSerializerUtility
{
    public static UnityObjectReference Serialize(UnityEngine.Object objectReference)
    {
        var obj = new JObject();
        if (objectReference is GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PrefabInstance prefabInstance))
            {
                return prefabInstance.GetReference();
            }

            return new UnityObjectReference();
        }

        return new UnityObjectReference();
    }
}

[Serializable]
public class UnityObjectReference
{
    public string path;
    public string guid;
    public long fileID = -1;
    
    // Scenarios:
    
    // (Project Scope) Reference a Unity AssetDatabase asset
    // { path: "assetDatabase", guid: "assetGuid", fileID: 12 }
    
    // (Project Scope) Reference an Object in the Database
    // { path: "images", guid: "assetGuid", fileID: -1 }
    
    // (Scene Scope) Reference an instance in scene
    // { path: "scenes", guid: "sceneGuid", fileID: 12 [, fileId2: 12] }
    // Note: fileId2 is needed if referencing a component of prefab instance
    
    // (Prefab Scope) Reference an instance in prefab
    // { path: "prefabs", guid: "prefabGuid", fileID: 12 }
    
    // It's likely prefab references in scene will 
}