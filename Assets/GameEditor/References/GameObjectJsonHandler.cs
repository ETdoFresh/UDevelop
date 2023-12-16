using ETdoFresh.ReadonlyInspectorAttribute;
using Unity.RuntimeSceneSerialization;
using UnityEngine;

namespace GameEditor.References
{
    public class GameObjectJsonHandler : MonoBehaviour
    {
        [SerializeField, TextArea(3,30)] private string json;
        [SerializeField, Readonly] private GameObject instance;

        private void OnEnable()
        {
            CreateInstance();
        }

        private void OnDisable()
        {
            DestroyInstance();
        }

        private void CreateInstance()
        {
            if (instance != null)
                Destroy(instance);
            
            if (!string.IsNullOrEmpty(json))
                instance = SceneSerialization.FromJson<GameObject>(json);
            
            if (instance != null)
                instance.transform.SetParent(transform, false);
        }
        
        private void DestroyInstance()
        {
            if (instance != null)
                Destroy(instance);
            
            instance = null;
        }
        
        public static GameObject CreateInstance(string json, GameObject parent = null)
        {
            if (!parent) parent = new GameObject(nameof(GameObjectJsonHandler));
            var handler = parent.GetComponent<GameObjectJsonHandler>();
            if (!handler) handler = parent.AddComponent<GameObjectJsonHandler>();
            handler.json = json;
            handler.CreateInstance();
            return handler.instance;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("ETdoFresh/Copy Selected Serialized To Clipboard")]
        private static void CopySelectedSerializedToClipboard()
        {
            var json = SceneSerialization.ToJson(UnityEditor.Selection.activeGameObject);
            GUIUtility.systemCopyBuffer = json;
            Debug.Log(json);
        }
#endif
    }
}