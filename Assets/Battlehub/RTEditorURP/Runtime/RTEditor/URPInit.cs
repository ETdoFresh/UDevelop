using Battlehub.RTCommon;
using Battlehub.UIControls;
using Battlehub.UIControls.DockPanels;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTEditor.URP
{
    public class URPInit : EditorExtension
    {
        [SerializeField]
        private GameObject m_prefab = null;
        [SerializeField, HideInInspector, /*Obsolete*/ ]
        private bool m_useForegroundLayerForUI = true;

        private GameObject m_instance;
        private GameObject m_foregroundOutput;

        protected override void OnInit()
        {
            base.OnInit();
        
            if(RenderPipelineInfo.Type != RPType.URP)
            {
                return;
            }

            if(!m_useForegroundLayerForUI)
            {
                RenderPipelineInfo.UseForegroundLayerForUI = false;
            }
            
            m_instance = Instantiate(m_prefab, transform, false);
            m_instance.name = m_prefab.name;

            IRTE rte = IOC.Resolve<IRTE>();
            IRTEAppearance appearance = IOC.Resolve<IRTEAppearance>();
            if (RenderPipelineInfo.UseForegroundLayerForUI)
            {
                Canvas foregroundCanvas = appearance.UIForegroundScaler.GetComponent<Canvas>();
                if (foregroundCanvas != null && foregroundCanvas.worldCamera != null)
                {
                    Camera foregroundCamera = foregroundCanvas.worldCamera;
                    GameObject foregroundLayer = new GameObject("ForegroundLayer");
                    foregroundLayer.transform.SetParent(rte.Root, false);
                    foregroundCanvas = foregroundLayer.AddComponent<Canvas>();
                    foregroundCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    foregroundCamera.gameObject.SetActive(false);
                    foregroundCamera.backgroundColor = new Color(0, 0, 0, 0);

                    m_foregroundOutput = new GameObject("Output");
                    m_foregroundOutput.transform.SetParent(foregroundCanvas.transform, false);
                    m_foregroundOutput.AddComponent<RectTransform>().Stretch();

                    RenderTextureCamera renderTextureCamera = foregroundCamera.gameObject.AddComponent<RenderTextureCamera>();
                    renderTextureCamera.OutputRoot = foregroundCanvas.gameObject.GetComponent<RectTransform>();
                    renderTextureCamera.Output = m_foregroundOutput.AddComponent<RawImage>();
                    renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/URP/RTEditor/UIForeground"));
                    foregroundCamera.gameObject.SetActive(true);

                    foregroundCanvas.sortingOrder = -1;
                }
            }
            else
            {
                Canvas foregroundCanvas = appearance.UIForegroundScaler != null ? appearance.UIForegroundScaler.GetComponent<Canvas>() : null;
                if(foregroundCanvas != null)
                {
                    if (foregroundCanvas.worldCamera != null)
                    {
                        Destroy(foregroundCanvas.worldCamera.gameObject);
                    }
                    foregroundCanvas.worldCamera = null;
                    foregroundCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    foregroundCanvas.sortingOrder = -1;
                }

                Canvas backgroundCanvas = appearance.UIBackgroundScaler != null ? appearance.UIBackgroundScaler.GetComponent<Canvas>() : null;
                if(backgroundCanvas != null)
                {
                    if (backgroundCanvas.worldCamera != null)
                    {
                        Destroy(backgroundCanvas.worldCamera.gameObject);
                        backgroundCanvas.worldCamera = null;
                        backgroundCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        backgroundCanvas.sortingOrder = -2;
                    }
                }

                IWindowManager wm = IOC.Resolve<IWindowManager>();
                DepthMaskingBehavior depthMaskingBehaviour = wm.ActiveWorkspace.DockPanel.GetComponent<DepthMaskingBehavior>();
                if(depthMaskingBehaviour != null)
                {
                    Destroy(depthMaskingBehaviour);
                }

                appearance.IsBackgroundImageEnabled = false;
            }
        }

        protected override void OnCleanup()
        {
            base.OnCleanup();
            Destroy(m_instance);
            m_instance = null;
        }
    }
}

