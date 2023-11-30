using DebuggingEssentials;
using ETdoFresh.UnityPackages.EventBusSystem;
using GameEditor.Selection.Events;
using UnityEngine;

namespace GameEditor.Selection
{
    public class SelectionChangeEventToEventBus : MonoBehaviour
    {
        private void OnEnable()
        {
            RuntimeInspector.onSelectedGOChanged += OnSelectedGOChanged;
        }

        private void OnDisable()
        {
            RuntimeInspector.onSelectedGOChanged -= OnSelectedGOChanged;
        }

        private void OnSelectedGOChanged(GameObject newSelection)
        {
            EventBus.Invoke(new SelectionChangedEvent { GameObject = newSelection });
        }
    }
}