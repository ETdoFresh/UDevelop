using UnityEngine;

namespace Jint
{
    public class JsScriptableObject : ScriptableObject
    {
        [SerializeField] private string javascript;
        [SerializeField] private int m_instanceID;
        [SerializeField] private int m_hashCode;

        public string JavaScript => javascript;
        public int InstanceID => m_instanceID == 0 ? m_instanceID = GetInstanceID() : m_instanceID;
        public int HashCode => m_hashCode = InstanceID.GetHashCode();

        public override int GetHashCode() => HashCode;
    }
}
