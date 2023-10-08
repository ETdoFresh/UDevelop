using ETdoFresh.UnityPackages.EventBusSystem;
using UnityEngine;

namespace CommandSystem
{
    public class CommandProcessor : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus.AddListener<CommandEvent>(OnCommandEvent);
        }
        
        private void OnDisable()
        {
            EventBus.RemoveListener<CommandEvent>(OnCommandEvent);
        }
        
        private void OnCommandEvent(CommandEvent e)
        {
            Debug.Log($"CommandProcessor: {e.Command}");
        }
    }
}