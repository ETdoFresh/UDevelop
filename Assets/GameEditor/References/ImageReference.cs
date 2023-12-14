using System;
using ETdoFresh.Localbase;
using GameEditor.Databases;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static ETdoFresh.Localbase.Paths;

namespace GameEditor.References
{
    public class ImageReference : MonoBehaviour
    {
        [SerializeField] private string guid;
        [SerializeField] private long tick;
        [SerializeField] private bool lockTickOnPublish = true;
        [SerializeField] private long bestTick;
        [SerializeField] private string bestTickString;
        [SerializeField] private string bestJsonString;
        private DatabaseReference _reference;
        private Texture2D _texture2D;

        private string EndPoint => $"{ImagesPath}.{guid}";
        
        private async void OnEnable()
        {
            if (string.IsNullOrEmpty(guid)) return;
            var value = await Database.GetValueAsync(EndPoint);
            if (value == null)
            {
                Debug.LogError($"[{nameof(ImageReference)}] Value is null");
                return;
            }

            var jObject = JObject.FromObject(value);
            if (tick == 0)
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOverNow(jObject);
                bestJsonString = DatabaseTickUtility.GetValueAtUtcNow(jObject).ToString();
            }
            else
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOver(jObject, tick);
                bestJsonString = DatabaseTickUtility.GetValueAtUtcTick(jObject, tick).ToString();
            }
            bestTickString = new DateTime(bestTick).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            
        }
    }
}
