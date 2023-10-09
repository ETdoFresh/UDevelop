using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using UDevelop.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UDevelop.UI.Behaviours
{
    public class UIPopupCloseButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private string popupName;
        [SerializeField] private Button button;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(popupName)) popupName = gameObject.name;
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
            EventBus.Invoke(new UIPopupEvent { PopupName = popupName, PopupType = UIPopupEvent.Type.Hide });
        }
    }
}