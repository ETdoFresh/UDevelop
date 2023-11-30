using ETdoFresh.UnityPackages.EventBusSystem;

namespace GameEditor.CodeEditor.Events
{
    public class CodeEditorAssetChangedEvent : EventBusEvent
    {
        public RuntimeCSharpAsset RuntimeCSharpAsset { get; set; }
    }
}