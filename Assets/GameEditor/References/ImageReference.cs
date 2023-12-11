using System;
using System.Linq;
using ETdoFresh.Localbase;
using GameEditor.Databases;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static ETdoFresh.Localbase.Paths;

namespace GameEditor.References
{
    public class Texture2DReference : MonoBehaviour
    {
        [SerializeField] private string guid;
        [SerializeField] private long tick;
        [SerializeField] private bool lockTickOnPublish = true;
        [SerializeField] private string bestTickString;
        [SerializeField] private string bestJsonString;
        private DatabaseReference _reference;
        private Texture2D _texture2D;

        private string EndPoint => $"{ImagesPath}.{guid}";
        
        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(guid))
                Database.ValueChanged.AddListener(EndPoint, OnValueChanged);
        }
        
        private void OnDisable()
        {
            if (!string.IsNullOrEmpty(guid))
                Database.ValueChanged.RemoveListener(EndPoint, OnValueChanged);
        }

        private void OnValueChanged(ValueChangedEventArgs e)
        {
            _reference.ValueChanged.RemoveListener(OnValueChanged);
            bestTickString = null;
            bestJsonString = null;
            var value = e.Snapshot.Value;
            if (value == null)
            {
                Debug.LogError($"[{nameof(Texture2DReference)}] Value is null");
                return;
            }
            var jObject = value as JObject;
            if (jObject == null)
            {
                Debug.LogError($"[{nameof(Texture2DReference)}] Value is not a JObject");
                return;
            }
            
            var properties = jObject.Properties().ToArray();
            if (properties.Length == 0)
            {
                Debug.LogError($"[{nameof(Texture2DReference)}] Value is an empty JObject");
                return;
            }
            
            var desiredTimestamp = tick == 0 ? DateTime.Now.Ticks : tick;
            var bestTick = 0L;
            var bestValue = (JToken)null;
            var maxTick = 0L;
            var maxValue = (JToken)null;
            foreach (var property in properties)
            {
                var currentTimestamp = long.Parse(property.Name);
                if (currentTimestamp > bestTick && currentTimestamp <= desiredTimestamp)
                {
                    bestTick = currentTimestamp;
                    bestValue = property.Value;
                }
                if (currentTimestamp > maxTick)
                {
                    maxTick = currentTimestamp;
                    maxValue = property.Value;
                }
            }
            if (bestValue == null && tick != 0)
            {
                Debug.LogError($"[{nameof(Texture2DReference)}] Could not find a value for timestamp {desiredTimestamp}");
                return;
            }
            if (bestValue == null)
            {
                Debug.LogWarning($"[{nameof(Texture2DReference)}] Could not find a value for timestamp {desiredTimestamp}, using latest value");
                bestValue = maxValue;
            }
            bestTickString = bestTick.ToString();
            bestJsonString = bestValue.ToString();
        }
    }
}
