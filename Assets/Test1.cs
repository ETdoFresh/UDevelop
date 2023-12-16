using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] private GameObject tempScene;
    [SerializeField] private bool testBool = true;
    [SerializeField] private int testInt = 1;
    [SerializeField] private float testFloat = 1.0f;
    [SerializeField] private string testString = "Hello World!";
    [SerializeField] private Vector3 testVector3 = Vector3.one;
    [SerializeField] private Quaternion testQuaternion = Quaternion.identity;
    [SerializeField] private UnityEngine.Object testObject;
    
    private async void OnEnable()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        Debug.Log("Hello World!");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        tempScene = new GameObject("TempScene");
        tempScene.SetActive(false);
        Debug.Log("Adding TempScene");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        var tempScene2 = new GameObject("TempScene2");
        tempScene2.transform.SetParent(tempScene.transform);
        var test2 = tempScene2.AddComponent<Test2>();
        test2.testString = "Hello World 2!";
        Debug.Log("TempScene2 added... activating TempScene in 1 second");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        tempScene.SetActive(true);
        Debug.Log("TempScene activated...");
    }

    private void OnDisable()
    {
        Debug.Log("Removing TempScene");
        Destroy(tempScene);
    }

    private class Test2 : MonoBehaviour
    {
        public string testString = "Hello World!";
        
        private void OnEnable()
        {
            Debug.Log(testString);
        }
    }
}
