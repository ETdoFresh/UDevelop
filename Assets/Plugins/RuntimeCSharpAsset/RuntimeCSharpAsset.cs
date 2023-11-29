using UnityEngine;
using UnityEngine.Events;

public class RuntimeCSharpAsset : ScriptableObject
{
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

    public void Save()
    {
        System.IO.File.WriteAllText(filePath, sourceCode);
        sourceChanged.Invoke();
    }
}