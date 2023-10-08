using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YAMLTest : MonoBehaviour
{
    public class HealthClass
    {
        public int[] healthArray;
        public int health;
        public float healthRegen;
        public float healthRegenDelay;
        public Dictionary<string, float> healthModifiers;
    }

    public class UnitySceneFile
    {
        
    }

    private void OnEnable()
    {
        var h = new HealthClass();
        h.health = 100;
        h.healthRegen = 0.5f;
        h.healthRegenDelay = 5.0f;
        h.healthModifiers = new Dictionary<string, float>();
        h.healthModifiers.Add("Fire", 1.0f);
        h.healthModifiers.Add("Ice", 2.0f);
        h.healthModifiers.Add("Poison", 3.0f);
        h.healthArray = new int[3];
        h.healthArray[0] = 1;
        h.healthArray[1] = 2;
        h.healthArray[2] = 3;
        
        var serializerBuilder = new YamlDotNet.Serialization.SerializerBuilder();
        var serializer = serializerBuilder.Build();
        var yaml = serializer.Serialize(h);
        Debug.Log(yaml);
        
        var deserializerBuilder = new YamlDotNet.Serialization.DeserializerBuilder();
        var deserializer = deserializerBuilder.Build();
        var h2 = deserializer.Deserialize<HealthClass>(yaml);
        Debug.Log($"health: {h2.health} healthRegen: {h2.healthRegen} healthRegenDelay: {h2.healthRegenDelay}");
    }
}
