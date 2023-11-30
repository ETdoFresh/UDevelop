using System;
using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using GameEditor.CodeEditor.Events;
using RuntimeCSharp;
using UnityEngine;
using TextEditor = InGameTextEditor.TextEditor;

namespace GameEditor.CodeEditor
{
    public class CodeEditorInputFieldBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeCSharpAsset runtimeCSharpAsset;
        [SerializeField] private TextEditor textEditor;
        private string _previousText;

        private void OnValidate()
        {
            if (!textEditor) textEditor = GetComponent<TextEditor>();
        }

        private void OnEnable()
        {
            EventBus.AddListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
            if (runtimeCSharpAsset) runtimeCSharpAsset.SourceChanged.AddPersistentListener(OnSourceChanged);
            OnSourceChanged();
        }

        private void OnDisable()
        {
            EventBus.RemoveListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
            if (runtimeCSharpAsset) runtimeCSharpAsset.SourceChanged.RemovePersistentListener(OnSourceChanged);
        }

        private void Update()
        {
            if (_previousText == textEditor.Text) return;
            _previousText = textEditor.Text;
            OnInputValueChanged(textEditor.Text);
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
                textEditor.SetText("\n\n    // No GameObject with a RuntimeCSharpBehaviour selected");
                textEditor.disableInput = true;
            }
            else
            {
                textEditor.SetText(runtimeCSharpAsset.sourceCode);
                textEditor.disableInput = false;
            }
        }
    
        private void OnInputValueChanged(string value)
        {
            if (runtimeCSharpAsset == null) return;
            runtimeCSharpAsset.sourceCode = value;
        }
    }
}
