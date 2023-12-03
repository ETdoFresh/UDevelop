using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScriptableObject : ScriptableObject
{
    public void LoadScene(string sceneName)
    {
        if (int.TryParse(sceneName, out var sceneIndex))
        {
            Debug.Log($"[{nameof(SceneLoaderScriptableObject)}] Loading scene index {sceneIndex}");
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.Log($"[{nameof(SceneLoaderScriptableObject)}] Loading scene {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
