using ETdoFresh.UnityPackages.EventBusSystem;
using UDevelop.UI.Events;
using UnityEngine;

namespace UDevelop.UI.Behaviours
{
    public class UIPopupBehaviour : MonoBehaviour
    {
        [SerializeField] private string popupName;
        [SerializeField] private GameObject popupContent;
        [SerializeField] private bool hideOnStart = true;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(popupName)) popupName = gameObject.name;
            if (!popupContent) popupContent = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            EventBus.AddListener<UIPopupEvent>(OnUIPopupEvent);
        }
    
        private void OnDisable()
        {
            EventBus.RemoveListener<UIPopupEvent>(OnUIPopupEvent);
        }
        
        private void Start()
        {
            if (hideOnStart) popupContent.SetActive(false);
        }
    
        private void OnUIPopupEvent(UIPopupEvent e)
        {
            if (e.PopupName != popupName) return;
            popupContent.SetActive(e.PopupType == UIPopupEvent.Type.Show);
        }
    }
}
