using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETdoFresh.SceneReferences
{
    public class SceneReferencesScriptableObject : ScriptableObject
    {
        private static bool _isSaving;
        
        [ContextMenu("Validate")]
        private void OnValidate()
        {
#if UNITY_EDITOR
            var buildSceneCount = SceneManager.sceneCountInBuildSettings;
            
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            var sceneReferences = subAssets.Where(x => x is SceneReference).Cast<SceneReference>().ToList();
            
            // Grow the list to match the build scene count
            for (var i = sceneReferences.Count; i < buildSceneCount; i++)
            {
                var sceneReference = CreateInstance<SceneReference>();
                sceneReferences.Add(sceneReference);
                AssetDatabase.AddObjectToAsset(sceneReference, this);
            }

            // Shrink the list to match the build scene count
            for (var i = sceneReferences.Count - 1; i >= buildSceneCount; i--)
            {
                AssetDatabase.RemoveObjectFromAsset(sceneReferences[i]);
                sceneReferences.RemoveAt(i);
            }
            
            // Get the build scene assets
            var buildSceneAssets = new SceneAsset[buildSceneCount];
            for (var i = 0; i < buildSceneCount; i++)
                buildSceneAssets[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneUtility.GetScenePathByBuildIndex(i));
            
            // Reassign sceneIndex for each sceneReference [if order has changed]
            for (var i = 0; i < buildSceneCount; i++)
            {
                var sceneAsset = buildSceneAssets[i];
                var sceneReference = sceneReferences.FirstOrDefault(x => x.sceneAsset == sceneAsset);
                if (sceneReference == null) continue;
                sceneReference.sceneIndex = i;
            }
            
            sceneReferences.Sort((a, b) => a.sceneIndex.CompareTo(b.sceneIndex));
            
            // Assign sceneName, sceneIndex, sceneAsset for each sceneReference
            for (var i = 0; i < sceneReferences.Count; i++)
            {
                var sceneAsset = buildSceneAssets[i];
                var sceneReference = sceneReferences[i];
                sceneReference.sceneIndex = i;
                sceneReference.sceneName = sceneAsset.name;
                sceneReference.sceneAsset = sceneAsset;
                sceneReference.name = sceneAsset.name;
            }
        
            // Prevent some possible infinite looping issues
            if (_isSaving) return;
            _isSaving = true;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            _isSaving = false;
#endif
        }
        
        public void LoadScene(SceneReference sceneReference)
        {
            Debug.Log($"[{nameof(SceneReferencesScriptableObject)}] Loading scene {sceneReference.sceneIndex} {sceneReference.sceneName}");
            SceneManager.LoadScene(sceneReference.sceneIndex);
        }
    }
}