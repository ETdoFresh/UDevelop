using TMPro;
using UnityEngine;

public class CodeEditorBehaviour : MonoBehaviour
{
    [SerializeField] private RuntimeCSharpAsset runtimeCSharpAsset;
    [SerializeField] private TMP_InputField inputField;
    
    private void OnEnable()
    {
        runtimeCSharpAsset.SourceChanged.AddListener(OnSourceChanged);
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        OnSourceChanged();
    }
    
    private void OnDisable()
    {
        runtimeCSharpAsset.SourceChanged.RemoveListener(OnSourceChanged);
        inputField.onValueChanged.RemoveListener(OnInputValueChanged);
    }
    
    private void OnSourceChanged()
    {
        inputField.text = runtimeCSharpAsset.sourceCode;
    }
    
    private void OnInputValueChanged(string value)
    {
        runtimeCSharpAsset.sourceCode = value;
    }
}
