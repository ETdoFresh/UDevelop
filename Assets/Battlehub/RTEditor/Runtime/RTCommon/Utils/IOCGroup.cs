using UnityEngine;
namespace Battlehub.RTCommon
{
    public class IOCGroup : MonoBehaviour
    {
        private IOCContainer m_container = new IOCContainer();
        public IOCContainer Container
        {
            get { return m_container; }
        }
    }
}