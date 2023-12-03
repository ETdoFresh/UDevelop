using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnSceneUnload : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    private void OnValidate()
    {
        sceneName = gameObject.scene.name;
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene unloadedScene)
    {
        if (unloadedScene.name == sceneName)
            Destroy(gameObject);
    }
}
