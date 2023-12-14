using System;
using ETdoFresh.Localbase;
using GameEditor.Databases;
using GameEditor.Organizations;
using GameEditor.Storages;
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
            var bestJObject = (JObject)null;
            if (tick == 0)
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOverNow(jObject);
                bestJObject = DatabaseTickUtility.GetValueAtUtcNow(jObject) as JObject;
                bestJsonString = bestJObject?.ToString();
            }
            else
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOver(jObject, tick);
                bestJObject = DatabaseTickUtility.GetValueAtUtcTick(jObject, tick) as JObject;
                bestJsonString = bestJObject?.ToString();
            }
            bestTickString = new DateTime(bestTick).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");

            var pathToTexture = bestJObject?.ToObject<ImageJsonObject>()?.path;
            if (!string.IsNullOrEmpty(pathToTexture))
            {
                _texture2D = new Texture2D(1, 1);
                _texture2D.LoadImage(await HttpCache.GetBytesAsync(pathToTexture));
                GetComponent<SpriteRenderer>().sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}
