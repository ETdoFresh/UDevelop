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

        [ContextMenu("Create Instance")]
        private void CreateInstance()
        {
            if (instance != null)
                Destroy(instance);
            
            if (!string.IsNullOrEmpty(json))
                instance = SceneSerialization.FromJson<GameObject>(json);
            
            if (instance != null)
                instance.transform.SetParent(transform, false);
        }
        
        [ContextMenu("Destroy Instance")]
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
            if (UnityEditor.Selection.activeObject == null) throw new System.Exception("No object selected");
            var activeObject = UnityEditor.Selection.activeObject;
            var activeObjectType = activeObject.GetType();
            var sceneSerializationToJsonMethod = typeof(SceneSerialization).GetMethod("ToJson");
            var sceneSerializationToJsonGenericMethod = sceneSerializationToJsonMethod.MakeGenericMethod(activeObjectType);
            var assetPackGuid = UnityEditor.AssetDatabase.FindAssets("t:AssetPack ETPack")[0];
            var assetPackPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assetPackGuid);
            var assetPack = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetPack>(assetPackPath);
            var serializationMetadata = new SerializationMetadata(assetPack);
            var json = (string) sceneSerializationToJsonGenericMethod.Invoke(null, new object[] {activeObject, serializationMetadata});
            GUIUtility.systemCopyBuffer = json;
            Debug.Log(json, UnityEditor.Selection.activeObject);
        }
        
        [UnityEditor.MenuItem("GameObject/Serialize To Clipboard", false, 0)]
        private static void SerializeToClipboard()
        {
            var json = SceneSerialization.ToJson(UnityEditor.Selection.activeGameObject);
            GUIUtility.systemCopyBuffer = json;
            Debug.Log(json);
        }
#endif
    }
    
    public static class GameObjectSerialization
    {
        public static string ToJson(GameObject go)
        {
            var json = SceneSerialization.ToJson(go);
            return json;
        }
        
        public static GameObject FromJson(string json)
        {
            var serializationMetadata = new SerializationMetadata
            {
                AssetPack = {  }
            };
            return SceneSerialization.FromJson<GameObject>(json);
        }
    }
}