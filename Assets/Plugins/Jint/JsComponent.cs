using System;
using Jint.Native;
using UnityEngine;

namespace Jint
{
    public class JsComponent : MonoBehaviour
    {
        [SerializeField] private JsScriptableObject js;
        [SerializeField] private string initialValues;
        private JsValue _componentJs;

        private void Awake()
        {
            _componentJs = JavaScript.Run(js.JavaScript);
            _componentJs.Invoke("awake");
        }
        
        private void Start()
        {
            _componentJs.Invoke("start");
        }

        private void OnEnable()
        {
            _componentJs.Invoke("onEnable");
        }
        
        private void OnDisable()
        {
            _componentJs.Invoke("onDisable");
        }

        private void Update()
        {
            _componentJs.Invoke("update");
        }
    }
}
