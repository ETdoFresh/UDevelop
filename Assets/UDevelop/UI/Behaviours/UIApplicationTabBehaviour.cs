using System;
using ETdoFresh.UnityPackages.EventBusSystem;
using UDevelop.UI.Events;
using UnityEngine;

namespace UDevelop.UI.Behaviours
{
    public class UIApplicationTabBehaviour : MonoBehaviour
    {
        [SerializeField] private string tabName;
        [SerializeField] private GameObject tabContent;
        [SerializeField] private bool hideOnStart = true;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(tabName)) tabName = gameObject.name;
            if (!tabContent && transform.childCount > 0) tabContent = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            EventBus.AddListener<ApplicationTabSelectEvent>(OnTabSelectEvent);
        }
    
        private void OnDisable()
        {
            EventBus.RemoveListener<ApplicationTabSelectEvent>(OnTabSelectEvent);
        }
        
        private void Start()
        {
            if (hideOnStart) tabContent.SetActive(false);
        }
    
        private void OnTabSelectEvent(TabSelectEvent e)
        {
            tabContent.SetActive(string.Equals(e.TabName, tabName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
