using System;
using System.Threading.Tasks;
using ETdoFresh.Localbase;
using GameEditor.Databases;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameEditor.References
{
    public class PrefabInstance : MonoBehaviour
    {
        [SerializeField] private string guid;
        [SerializeField] private long tick;
        [SerializeField] private bool lockTickOnPublish = true;
        
        [Header("Read Only/Debug")]
        [SerializeField] private long bestTick;
        [SerializeField] private string bestTickString;
        [SerializeField] private string bestJsonString;

        private string EndPoint => $"{Paths.PrefabsPath}.{guid}";

        private async Task OnEnable()
        {
            if (string.IsNullOrEmpty(guid)) return;
            var jObject = await Database.GetValueAsync<JObject>(EndPoint);
            if (jObject == null)
            {
                Debug.LogError($"[{nameof(PrefabReference)}] Value is null");
                return;
            }
            
            JObject bestJObject;
            if (tick == 0)
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOverNow(jObject);
                bestJObject = DatabaseTickUtility.GetValueAtUtcNow<JObject>(jObject);
                bestJsonString = bestJObject?.ToString();
            }
            else
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOver(jObject, tick);
                bestJObject = DatabaseTickUtility.GetValueAtUtcTick<JObject>(jObject, tick);
                bestJsonString = bestJObject?.ToString();
            }
            bestTickString = new DateTime(bestTick).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}