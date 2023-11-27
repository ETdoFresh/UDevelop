#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles.URP
{
    public static class RTHandlesMenu
    {
        [MenuItem("Tools/Runtime Handles/Add URP support", priority = 50)]
        public static void AddURPSupport()
        {
            GameObject urpSupport = GameObject.Find("RTHandles URP Support");
            if (urpSupport)
            {
                Selection.activeGameObject = urpSupport;
                EditorGUIUtility.PingObject(urpSupport);
            }
            else
            {
                urpSupport = InstantiateURPSupport();
                urpSupport.name = "RTHandles URP Support";

                if (urpSupport != null)
                {
                    Undo.RegisterCreatedObjectUndo(urpSupport, "Battlehub.RTHandles.URPSupport");
                }
            }

            if (GraphicsSettings.renderPipelineAsset == null ||
                GraphicsSettings.renderPipelineAsset.name != "HighQuality_UniversalRenderPipelineAsset" &&
                GraphicsSettings.renderPipelineAsset.name != "MidQuality_UniversalRenderPipelineAsset" &&
                GraphicsSettings.renderPipelineAsset.name != "LowQuality_UniversalRenderPipelineAsset")
            {
                SuggestToUseBuiltinRenderPipelineAsset();
            }
        }

        [MenuItem("Tools/Runtime Handles/Use built-in RenderPipelineAsset", priority = 49)]
        public static void SuggestToUseBuiltinRenderPipelineAsset()
        {
            if (EditorUtility.DisplayDialog("Confirmation of change of the rendering pipeline asset", "Use built-in HighQuality_UniversalRenderPipelineAsset?", "Yes", "No"))
            {
                GraphicsSettings.renderPipelineAsset = Resources.Load<RenderPipelineAsset>("HighQuality_UniversalRenderPipelineAsset");
                QualitySettings.renderPipeline = GraphicsSettings.renderPipelineAsset;
            }
        }

        public static GameObject InstantiateURPSupport()
        {
            UnityObject prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/URP.prefab", typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}
#endif