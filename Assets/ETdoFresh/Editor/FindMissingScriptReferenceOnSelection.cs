#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETdoFresh.Editor
{
    public static class FindMissingScriptReferenceOnSelection
    {
        [MenuItem("ETdoFresh/Find Missing Script Reference On Selection")]
        public static void FindMissingScriptReference()
        {
            var selection = new List<GameObject>(Selection.gameObjects);
            foreach(var gameObject in Selection.gameObjects)
                if (FindMissingScriptReference(gameObject.transform))
                    return;
            UnityEngine.Debug.Log($"[FindMissingScriptReferenceOnSelection] No missing script references found on selection:\n{string.Join("\n", selection)}");
        }

        private static bool FindMissingScriptReference(Transform transform)
        {
            foreach(var component in transform.GetComponents<Component>())
                if (!component)
                {
                    UnityEngine.Debug.LogError($"Missing Component on GameObject: {transform.name}", transform.gameObject);
                    Selection.activeGameObject = transform.gameObject;
                    return true;
                }
            
            foreach(Transform child in transform)
                if (FindMissingScriptReference(child))
                    return true;
            
            return false;
        }
    }
}
#endif