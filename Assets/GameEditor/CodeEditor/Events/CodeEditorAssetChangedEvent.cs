using ETdoFresh.UnityPackages.EventBusSystem;
using RuntimeCSharp;

namespace GameEditor.CodeEditor.Events
{
    public class CodeEditorAssetChangedEvent : EventBusEvent
    {
        public RuntimeCSharpAsset RuntimeCSharpAsset { get; set; }
    }
}