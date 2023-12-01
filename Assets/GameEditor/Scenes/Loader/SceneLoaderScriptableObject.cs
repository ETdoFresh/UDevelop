using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScriptableObject : ScriptableObject
{
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[{nameof(SceneLoaderScriptableObject)}] Loading scene {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
