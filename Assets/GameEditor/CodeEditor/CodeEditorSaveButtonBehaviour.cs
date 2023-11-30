using System;
using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using GameEditor.CodeEditor.Events;
using RuntimeCSharp;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.CodeEditor
{
    public class CodeEditorSaveButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeCSharpAsset currentAsset;
        [SerializeField] private Button button;

        private void OnValidate()
        {
            if (!button) button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddPersistentListener(OnButtonClicked);
            EventBus.AddListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
        }
        
        private void OnDisable()
        {
            button.onClick.RemovePersistentListener(OnButtonClicked);
            EventBus.RemoveListener<CodeEditorAssetChangedEvent>(OnCodeEditorAssetChanged);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) 
                OnButtonClicked();
        }

        private void OnButtonClicked()
        {
            if (currentAsset == null) return;
            currentAsset.Save();
        }
        
        private void OnCodeEditorAssetChanged(CodeEditorAssetChangedEvent e)
        {
            currentAsset = e.RuntimeCSharpAsset;
            button.interactable = currentAsset != null;
        }
    }
}