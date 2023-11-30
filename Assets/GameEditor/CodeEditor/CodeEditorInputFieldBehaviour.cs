using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using GameEditor.CodeEditor.Events;
using TMPro;
using UnityEngine;

namespace GameEditor.CodeEditor
{
    public class CodeEditorInputFieldBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeCSharpAsset runtimeCSharpAsset;
        [SerializeField] private TMP_InputField inputField;
    
        private void OnEnable()
        {
            EventBus.AddListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
            if (runtimeCSharpAsset) runtimeCSharpAsset.SourceChanged.AddPersistentListener(OnSourceChanged);
            inputField.onValueChanged.AddPersistentListener(OnInputValueChanged);
            OnSourceChanged();
        }

        private void OnDisable()
        {
            EventBus.RemoveListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
            if (runtimeCSharpAsset) runtimeCSharpAsset.SourceChanged.RemovePersistentListener(OnSourceChanged);
            inputField.onValueChanged.RemovePersistentListener(OnInputValueChanged);
        }

        private void OnCodeEditorAssetChanged(CodeEditorAssetChangedEvent e)
        {
            runtimeCSharpAsset = e.RuntimeCSharpAsset;
            if (runtimeCSharpAsset) runtimeCSharpAsset.SourceChanged.AddPersistentListener(OnSourceChanged);
            OnSourceChanged();
        }

        private void OnSourceChanged()
        {
            if (runtimeCSharpAsset == null)
            {
                inputField.interactable = false;
                inputField.text = "\n\n    // No asset selected";
            }
            else
            {
                inputField.interactable = true;
                inputField.text = runtimeCSharpAsset.sourceCode;
            }
        }
    
        private void OnInputValueChanged(string value)
        {
            if (runtimeCSharpAsset == null) return;
            runtimeCSharpAsset.sourceCode = value;
        }
    }
}
