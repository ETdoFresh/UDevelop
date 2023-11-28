#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ETdoFresh
{
    public static class ResetSceneCamera
    {
        [MenuItem("ETdoFresh/Reset Scene Camera (2D)")]
        public static void ResetView()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                Debug.LogWarning("No scene view found");
                return;
            }
            
            sceneView.pivot = Vector3.zero;
            sceneView.rotation = Quaternion.identity;
            sceneView.size = 10f;
            sceneView.orthographic = true;
            sceneView.Repaint();
        }
        
        [MenuItem("ETdoFresh/Reset Scene Camera (3D)")]
        public static void ResetView3D()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                Debug.LogWarning("No scene view found");
                return;
            }
            
            sceneView.pivot = Vector3.zero;
            sceneView.rotation = Quaternion.Euler(26.33f, -135f, 0f);
            sceneView.size = 10f;
            sceneView.orthographic = false;
            sceneView.Repaint();
        }
    }
}
#endif