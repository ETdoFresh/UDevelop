using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Battlehub.RTCommon.URP
{
    public class RenderCache : ScriptableRendererFeature
    {
        [System.Serializable]
        public class RenderCacheSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
            public string RenderersCacheName = "RenderersCache";
        }

        [SerializeField]
        public RenderCacheSettings m_settings = new RenderCacheSettings();

        private class RenderCachePass : ScriptableRenderPass
        {
            private IRenderersCache m_renderersCache;
            public void Setup(IRenderersCache renderersCache)
            {
                m_renderersCache = renderersCache;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("RenderCache");

                if (m_renderersCache != null)
                {
                    IList<Renderer> renderers = m_renderersCache.Renderers;
                    for (int i = 0; i < renderers.Count; ++i)
                    {
                        Renderer renderer = renderers[i];
                        if (renderer != null && renderer.enabled && renderer.gameObject.activeSelf)
                        {
                            Material[] materials = renderer.sharedMaterials;

                            for (int j = 0; j < materials.Length; ++j)
                            {
                                if (materials[j] != null)
                                {
                                    cmd.DrawRenderer(renderer, materials[j], j);
                                }
                            }
                        }
                    }
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        private RenderCachePass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new RenderCachePass();
            m_ScriptablePass.renderPassEvent = m_settings.Event;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            IRenderersCache renderersCache = IOC.Resolve<IRenderersCache>(m_settings.RenderersCacheName);
            if ((renderersCache == null || renderersCache.IsEmpty))
            {
                return;
            }

            m_ScriptablePass.Setup(renderersCache);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }



}
