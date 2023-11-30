using ETdoFresh.UnityPackages.EventBusSystem;
using GameEditor.CodeEditor.Events;
using GameEditor.Selection.Events;
using RuntimeCSharp;
using UnityEngine;

namespace GameEditor.CodeEditor
{
    public class CodeEditorSelectionEventToCodeEditorEvent : MonoBehaviour
    {
        [SerializeField] private RuntimeCSharpAsset currentAsset;
        
        private void OnEnable()
        {
            EventBus.AddListener<SelectionChangedEvent>(OnSelectionChanged);
        }
        
        private void OnDisable()
        {
            EventBus.RemoveListener<SelectionChangedEvent>(OnSelectionChanged);
        }
        
        private void OnSelectionChanged(SelectionChangedEvent e)
        {
            var newSelectedGameObject = e.GameObject;
            if (newSelectedGameObject == null)
            {
                SetCurrentAsset(null);
                return;
            }
            
            var runtimeCSharpBehaviour = newSelectedGameObject.GetComponent<RuntimeCSharpBehaviour>();
            if (runtimeCSharpBehaviour == null)
            {
                SetCurrentAsset(null);
                return;
            }
            
            SetCurrentAsset(runtimeCSharpBehaviour.RuntimeCSharpAsset);
        }
        
        private void SetCurrentAsset(RuntimeCSharpAsset asset)
        {
            currentAsset = asset;
            EventBus.Invoke(new CodeEditorAssetChangedEvent { RuntimeCSharpAsset = asset });
        }
    }
}