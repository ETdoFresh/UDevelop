#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace Battlehub.RTEditor.URP
{
    public static class RTExtensionsMenu
    {
        [MenuItem("Tools/Runtime Editor/Add URP support for RTExtensions", priority = 51)]
        public static void CreateExtensionsURP()
        {
            GameObject urpSupport = GameObject.Find("RTExtensions URP Support");
            if (urpSupport != null)
            {
                Selection.activeGameObject = urpSupport;
                EditorGUIUtility.PingObject(urpSupport);
            }
            else
            {
                urpSupport = InstantiateURPSupport();
                urpSupport.name = "RTExtensions URP Support";
                Undo.RegisterCreatedObjectUndo(urpSupport, "Battlehub.RTExtensions.CreateURP");
            }
        }

        public static GameObject InstantiateURPSupport()
        {
            UnityObject prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/EditorExtensionsURP.prefab", typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}
#endif