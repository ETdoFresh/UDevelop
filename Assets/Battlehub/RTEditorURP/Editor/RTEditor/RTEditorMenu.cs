#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityEngine.Rendering;

namespace Battlehub.RTEditor.URP
{
    public static class RTEditorMenu
    {
        [MenuItem("Tools/Runtime Editor/Add URP support for RTEditor", priority = 50)]
        public static void AddURPSupport()
        {
            GameObject urpSupport = GameObject.Find("RTEditor URP Support");
            if (urpSupport)
            {
                Selection.activeGameObject = urpSupport;
                EditorGUIUtility.PingObject(urpSupport);
            }
            else
            {
                urpSupport = InstantiateURPSupport();
                urpSupport.name = "RTEditor URP Support";

                if (urpSupport != null)
                {
                    Undo.RegisterCreatedObjectUndo(urpSupport, "Battlehub.RTEditor.URPSupport");
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

        [MenuItem("Tools/Runtime Editor/Use built-in RenderPipelineAsset", priority = 49)]
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
            UnityObject prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/RTEditorInitURP.prefab", typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}
#endif