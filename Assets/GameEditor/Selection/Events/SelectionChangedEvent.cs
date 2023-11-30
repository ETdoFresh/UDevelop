using ETdoFresh.UnityPackages.EventBusSystem;
using UnityEngine;

namespace GameEditor.Selection.Events
{
    public class SelectionChangedEvent : EventBusEvent
    {
        public GameObject GameObject { get; set; }
    }
}