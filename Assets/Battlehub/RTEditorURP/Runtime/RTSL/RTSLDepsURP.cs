using Battlehub.RTCommon;
using Battlehub.Utils;
using Battlehub.Utils.URP;
using UnityEngine;

namespace Battlehub.RTSL.URP
{
    [DefaultExecutionOrder(-90)]
    public class RTSLDepsURP : MonoBehaviour
    {
        private IMaterialUtil m_materialUtils;

        private void Awake()
        {
            m_materialUtils = new URPLitMaterialUtils();
            IOC.Register(m_materialUtils);
        }

        private void OnDestroy()
        {
            IOC.Unregister(m_materialUtils);
        }
    }

}
