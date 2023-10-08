using System;
using ETdoFresh.UnityPackages.EventBusSystem;
using ETdoFresh.UnityPackages.ExtensionMethods;
using TMPro;
using UnityEngine;

namespace CommandSystem
{
    public class CommandInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        private void OnValidate()
        {
            if (!inputField) inputField = GetComponent<TMP_InputField>();
        }

        private void OnEnable()
        {
            inputField.onSubmit.AddPersistentListener(OnSubmit);
        }
    
        private void OnDisable()
        {
            inputField.onSubmit.RemovePersistentListener(OnSubmit);
        }

        private void OnSubmit(string command)
        {
            EventBus.Invoke(new CommandEvent { Command = command });
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}