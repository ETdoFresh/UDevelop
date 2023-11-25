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
            if (js) js.ScriptChanged.AddListener(HotReload);
            HotReload();
            _componentJs?.AsObject()?.GetProperty("awake")?.Value?.Invoke();
        }
        
        private void Start()
        {
            _componentJs?.AsObject()?.GetProperty("start")?.Value?.Invoke();
        }
        
        private void OnDestroy()
        {
            _componentJs?.AsObject()?.GetProperty("onDestroy")?.Value?.Invoke();
            if (js) js.ScriptChanged.RemoveListener(HotReload);
        }

        private void OnEnable()
        {
            _componentJs?.AsObject()?.GetProperty("onEnable")?.Value?.Invoke();
        }
        
        private void OnDisable()
        {
            _componentJs?.AsObject()?.GetProperty("onDisable")?.Value?.Invoke();
        }
        
        private void FixedUpdate()
        {
            _componentJs?.AsObject()?.GetProperty("fixedUpdate")?.Value?.Invoke();
        }
        
        private void LateUpdate()
        {
            _componentJs?.AsObject()?.GetProperty("lateUpdate")?.Value?.Invoke();
        }

        private void Update()
        {
            _componentJs?.AsObject()?.GetProperty("update")?.Value?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionEnter")?.Value?.Invoke(collisionJs);
        }
        
        private void OnCollisionExit(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionExit")?.Value?.Invoke(collisionJs);
        }
        
        private void OnCollisionStay(Collision other)
        {
            var collisionJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onCollisionStay")?.Value?.Invoke(collisionJs);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerEnter")?.Value?.Invoke(colliderJs);
        }
        
        private void OnTriggerExit(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerExit")?.Value?.Invoke(colliderJs);
        }
        
        private void OnTriggerStay(Collider other)
        {
            var colliderJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onTriggerStay")?.Value?.Invoke(colliderJs);
        }
        
        private void OnParticleCollision(GameObject other)
        {
            var gameObjectJs = JavaScript.GetJsValue(other);
            _componentJs?.AsObject()?.GetProperty("onParticleCollision")?.Value?.Invoke(gameObjectJs);
        }

        [ContextMenu("Hot Reload")]
        private void HotReload()
        {
            var selfJs = JavaScript.GetJsValue(this);
            JavaScript.Engine.SetValue("self", selfJs);
            _componentJs = JavaScript.Evaluate(js.JavaScript);
            JavaScript.Evaluate($"delete self;");
        }
        
        public void SetProperty(string propertyPath, object value)
        {
            const string tempName = "super_unique_name_that_will_never_collide_with_anything";
            JavaScript.SetValue(tempName, value);
            JavaScript.Evaluate($@"{propertyPath} = {tempName}; delete globalThis.{tempName};");
        }
    }
}
