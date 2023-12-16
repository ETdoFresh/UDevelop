using System.Threading.Tasks;
using ETdoFresh.Localbase;
using GameEditor.Databases;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameEditor.References
{
    public class PrefabReference : MonoBehaviour
    {
        [SerializeField] private string guid;
        [SerializeField] private long tick;
        [SerializeField] private bool lockTickOnPublish = true;
        [SerializeField] private long bestTick;
        [SerializeField] private string bestTickString;
        [SerializeField] private string bestJsonString;
        [SerializeField] private GameObject gameObjectToCapture;
        [SerializeField] private GameObject newObject;
        private JObject _jObject;
        private GameObject _prefab;

        private string EndPoint => $"{Paths.PrefabsPath}.{guid}";

        private async Task OnEnable()
        {
            if (string.IsNullOrEmpty(guid)) return;
            _jObject = await Database.GetValueAsync<JObject>(EndPoint);
            if (_jObject == null)
            {
                Debug.LogError($"[{nameof(PrefabReference)}] Value is null");
                return;
            }
            
            JObject bestJObject;
            if (tick == 0)
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOverNow(_jObject);
                bestJObject = DatabaseTickUtility.GetValueAtUtcNow<JObject>(_jObject);
                bestJsonString = bestJObject?.ToString();
            }
            else
            {
                bestTick = DatabaseTickUtility.GetClosestTickWithoutGoingOver(_jObject, tick);
                bestJObject = DatabaseTickUtility.GetValueAtUtcTick<JObject>(_jObject, tick);
                bestJsonString = bestJObject?.ToString();
            }
        }
    }
}