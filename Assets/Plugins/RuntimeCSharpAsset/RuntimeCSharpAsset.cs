using System;
using UnityEngine;
using UnityEngine.Events;

public class RuntimeCSharpAsset : ScriptableObject
{
    private const string PersistentDataPath = "CSharpSourceCode";
    
    [SerializeField] public string filePath;
    [SerializeField, TextArea(3, 50)] public string sourceCode;
    [SerializeField] private UnityEvent sourceChanged = new();
    private string _previousSourceCode;

    public UnityEvent SourceChanged => sourceChanged;

    public void OnValidate()
    {
#if UNITY_EDITOR
        if (_previousSourceCode == sourceCode) return;
        _previousSourceCode = sourceCode;
        sourceChanged.Invoke();
#endif
    }

    private void Awake()
    {
#if !UNITY_EDITOR
        var filename = System.IO.Path.GetFileName(filePath);
        var path = System.IO.Path.Combine(Application.persistentDataPath, PersistentDataPath, filename);
        if (System.IO.File.Exists(path) == false) return;
        sourceCode = System.IO.File.ReadAllText(path);
        Debug.Log($"Loaded from {path}");
#endif
    }

    public void Save()
    {
#if UNITY_EDITOR
        System.IO.File.WriteAllText(filePath, sourceCode);
#else
        var filename = System.IO.Path.GetFileName(filePath);
        // Save to persistent data path
        var directory = System.IO.Path.Combine(Application.persistentDataPath, PersistentDataPath);
        if (System.IO.Directory.Exists(directory) == false)
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        var path = System.IO.Path.Combine(directory, filename);
        System.IO.File.WriteAllText(path, sourceCode);
        Debug.Log($"Saved to {path}");
#endif
        sourceChanged.Invoke();
    }
}