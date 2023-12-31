using UnityEngine;
using UnityEngine.Events;

namespace Jint
{
    public class JsScriptableObject : ScriptableObject
    {
        [SerializeField] private string javascript;
        [SerializeField] private int m_instanceID;
        [SerializeField] private int m_hashCode;
        [SerializeField] private UnityEvent scriptChanged = new();
        private string _previousJavaScript;

        public string JavaScript { get => javascript; set => javascript = value; }
        public int InstanceID => m_instanceID == 0 ? m_instanceID = GetInstanceID() : m_instanceID;
        public int HashCode => m_hashCode = InstanceID.GetHashCode();
        public UnityEvent ScriptChanged => scriptChanged;

        public override int GetHashCode() => HashCode;

        public void OnValidate()
        {
#if UNITY_EDITOR
            if (_previousJavaScript == javascript) return;
            _previousJavaScript = javascript;
            scriptChanged.Invoke();
#endif
        }
    }
}