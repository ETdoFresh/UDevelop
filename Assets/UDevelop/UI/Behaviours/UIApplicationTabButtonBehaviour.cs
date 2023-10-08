using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using UDevelop.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UDevelop.UI.Behaviours
{
    public class UIApplicationTabButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private string tabName;
        [SerializeField] private Button button;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(tabName)) tabName = gameObject.name;
            if (!button) button = GetComponent<Button>();
        }
        
        private void OnEnable()
        {
            button.onClick.AddPersistentListener(OnButtonClick);
        }
        
        private void OnDisable()
        {
            button.onClick.RemovePersistentListener(OnButtonClick);
        }
        
        private void OnButtonClick()
        {
            EventBus.Invoke(new ApplicationTabSelectEvent { TabName = tabName });
        }
    }
}