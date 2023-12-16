using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class GameObjectSerializerUtility
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = new List<JsonConverter>
        {
            new Vector3JsonConverter(),
            new QuaternionJsonConverter(),
            new TransformJsonConverter(),
            new GameObjectJsonConverter(),
            new CameraJsonConverter(),
            new UniversalAdditionalCameraDataJsonConverter(),
            new BehaviourJsonConverter(),
            new ComponentJsonConverter(),
        }
    };
    
    public static string Serialize(Object objectToSerialize)
    {
        return JsonConvert.SerializeObject(objectToSerialize, Settings);
    }
}