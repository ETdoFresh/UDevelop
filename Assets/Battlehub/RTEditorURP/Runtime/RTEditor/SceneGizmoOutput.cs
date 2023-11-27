using Battlehub.RTCommon;
using Battlehub.RTHandles;
using Battlehub.UIControls;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTEditor.URP
{
    /*
    public class SceneGizmoOutput : MonoBehaviour
    {
        [SerializeField]
        private SceneGizmo m_sceneGizmo = null;
        private Canvas m_sourceCanvas;

        public SceneGizmo SceneGizmo
        {
            get { return m_sceneGizmo; }
            set { m_sceneGizmo = value; }
        }

        private RectTransform m_rectTransform;
        
        private void OnEnable()
        {
            if(m_sceneGizmo == null)
            {
                enabled = false;
                return;
            }

            m_sourceCanvas = m_sceneGizmo.m_output.GetComponentInParent<Canvas>();
            m_rectTransform = GetComponent<RectTransform>();
            if(m_rectTransform == null)
            {
                m_rectTransform = gameObject.AddComponent<RectTransform>();
            }

            GameObject output = new GameObject("Output");
            RectTransform outputRectTransform = output.AddComponent<RectTransform>();
            outputRectTransform.transform.SetParent(m_rectTransform, false);
            outputRectTransform.Stretch();
            outputRectTransform.pivot = Vector2.zero;

            RenderTextureCamera renderTextureCamera = SceneGizmo.GetComponent<RenderTextureCamera>();
            renderTextureCamera.Output = output.AddComponent<RawImage>();
            renderTextureCamera.OutputRoot = renderTextureCamera.Output.rectTransform;

            m_rectTransform.anchorMax = Vector2.zero;
            m_rectTransform.anchorMin = Vector2.zero;
            m_rectTransform.pivot = m_sceneGizmo.m_output.pivot;
            m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_sceneGizmo.m_output.rect.width);
            m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_sceneGizmo.m_output.rect.height);

        }

        private void Update()
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(m_sourceCanvas.worldCamera, m_sceneGizmo.m_output.position);
            m_rectTransform.anchoredPosition = screenPoint;
        }
    }
    */
}


