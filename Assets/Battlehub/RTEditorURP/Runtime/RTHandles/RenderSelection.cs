using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Battlehub.RTHandles.URP
{
    public class RenderSelection : ScriptableRendererFeature
    {
        [System.Serializable]
        public class RenderSelectionSettings
        {
            public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Material PrepassMaterial = null;
            public Material BlurMaterial = null;
            public Material CompositeMaterial = null;
            public Color OutlineColor = new Color32(255, 128, 0, 255);
            public string MeshesCacheName = "SelectedMeshes";
            public string RenderersCacheName = "SelectedRenderers";
            public string CustomRenderersCacheName = "CustomOutlineRenderersCache";
            public LayerMask LayerMask = -1;

            [Range(0.5f, 10f)]
            public float OutlineStength = 5;

            [Range(0.1f, 3)]
            public float BlurSize = 1f;
        }

        [SerializeField]
        public RenderSelectionSettings m_settings = new RenderSelectionSettings();

        class RenderSelectionPass : ScriptableRenderPass
        {
            public RenderSelectionSettings Settings;

            private IMeshesCache m_meshesCache;
            private IRenderersCache m_renderersCache;
            private ICustomOutlineRenderersCache m_customRenderersCache;

            private int m_prepassId;
            private RenderTargetIdentifier m_prepassRT;
            private RTHandle m_prepassHandle;

            private int m_blurredId;
            private RenderTargetIdentifier m_blurredRT;

            private int m_tmpTexId;
            private RenderTargetIdentifier m_tmpRT;


#if UNIVERSAL_RP_15_0_OR_NEWER
            private RTHandle m_cameraColorRT;
#else
            private RenderTargetIdentifier m_cameraColorRT;
#endif
            private int m_outlineColorId;
            private int m_outlineStrengthId;
            private int m_blurDirectionId;

#if UNIVERSAL_RP_15_0_OR_NEWER
            public void Setup(RTHandle camerColorRT, IMeshesCache meshesCache, IRenderersCache renderersCache, ICustomOutlineRenderersCache customRenderersCache)
            {
                m_meshesCache = meshesCache;
                m_renderersCache = renderersCache;
                m_customRenderersCache = customRenderersCache;
                m_cameraColorRT = camerColorRT;
            }
#else
            public void Setup(RenderTargetIdentifier camerColorRT, IMeshesCache meshesCache, IRenderersCache renderersCache, ICustomOutlineRenderersCache customRenderersCache)
            {
                m_meshesCache = meshesCache;
                m_renderersCache = renderersCache;
                m_customRenderersCache = customRenderersCache;
                m_cameraColorRT = camerColorRT;
            }
#endif

            private RenderTextureDescriptor GetStereoCompatibleDescriptor(RenderTextureDescriptor descriptor, int width, int height, GraphicsFormat format, int depthBufferBits = 0)
            {
                // Inherit the VR setup from the camera descriptor
                var desc = descriptor;
                desc.depthBufferBits = depthBufferBits;
                desc.msaaSamples = 1;
                desc.width = width;
                desc.height = height;
                desc.graphicsFormat = format;
                return desc;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor camDesc)
            {
                base.Configure(cmd, camDesc);
#if UNIVERSAL_RP_15_0_OR_NEWER
                if (m_cameraColorRT == null)
                {
                    return;
                }
#endif
                var width = camDesc.width;
                var height = camDesc.height;

                m_prepassId = Shader.PropertyToID("_PrepassTex");
                m_blurredId = Shader.PropertyToID("_BlurredTex");
                m_tmpTexId = Shader.PropertyToID("_TmpTex");
                m_outlineColorId = Shader.PropertyToID("_OutlineColor");
                m_outlineStrengthId = Shader.PropertyToID("_OutlineStrength");
                m_blurDirectionId = Shader.PropertyToID("_BlurDirection");

                var desc = GetStereoCompatibleDescriptor(camDesc, width, height, camDesc.graphicsFormat);
                cmd.GetTemporaryRT(m_prepassId, desc);
                cmd.GetTemporaryRT(m_blurredId, desc);
                cmd.GetTemporaryRT(m_tmpTexId, desc);

                m_prepassRT = new RenderTargetIdentifier(m_prepassId);
                m_blurredRT = new RenderTargetIdentifier(m_blurredId);
                m_tmpRT = new RenderTargetIdentifier(m_tmpTexId);

                m_prepassHandle = UnityEngine.Rendering.RTHandles.Alloc(m_prepassRT);
#if UNIVERSAL_RP_15_0_OR_NEWER
                ConfigureTarget(m_prepassHandle);
#else
                ConfigureTarget(m_prepassRT);
#endif
                ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 1));
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("RenderSelection");
                bool draw = false;

                if (m_meshesCache != null)
                {
                    IList<RenderMeshesBatch> batches = m_meshesCache.Batches;
                    for (int i = 0; i < batches.Count; ++i)
                    {
                        RenderMeshesBatch batch = batches[i];
                        for (int j = 0; j < batch.Mesh.subMeshCount; ++j)
                        {
                            if (batch.Mesh != null)
                            {
                                cmd.DrawMeshInstanced(batch.Mesh, j, Settings.PrepassMaterial, 0, batch.Matrices, batch.Matrices.Length);
                                draw = true;
                            }
                        }
                    }
                }

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
                                    cmd.DrawRenderer(renderer, Settings.PrepassMaterial, j);
                                    draw = true;
                                }
                            }
                        }
                    }
                }

                if (m_customRenderersCache != null)
                {
                    List<ICustomOutlinePrepass> renderers = m_customRenderersCache.GetOutlineRendererItems();
                    for (int i = 0; i < renderers.Count; ++i)
                    {
                        ICustomOutlinePrepass renderer = renderers[i];
                        if (renderer != null && renderer.GetRenderer().gameObject.activeSelf)
                        {
                            Material[] materials = renderer.GetRenderer().sharedMaterials;

                            for (int j = 0; j < materials.Length; ++j)
                            {
                                if (materials[j] != null)
                                {
                                    cmd.DrawRenderer(renderer.GetRenderer(), renderer.GetOutlinePrepassMaterial(), j);
                                    draw = true;
                                }
                            }
                        }
                    }
                }

                if (draw)
                {
                    cmd.Blit(m_prepassRT, m_blurredRT);
                    cmd.SetGlobalFloat(m_outlineStrengthId, Settings.OutlineStength);
                    cmd.SetGlobalVector(m_blurDirectionId, new Vector2(Settings.BlurSize, 0));
                    cmd.Blit(m_blurredRT, m_tmpRT, Settings.BlurMaterial, 0);
                    cmd.SetGlobalVector(m_blurDirectionId, new Vector2(0, Settings.BlurSize));
                    cmd.Blit(m_tmpRT, m_blurredRT, Settings.BlurMaterial, 0);

                    cmd.Blit(m_cameraColorRT, m_tmpRT);
                    cmd.SetGlobalTexture(m_prepassId, m_prepassRT);
                    cmd.SetGlobalTexture(m_blurredId, m_blurredId);
                    cmd.SetGlobalColor(m_outlineColorId, Settings.OutlineColor);
                    cmd.Blit(m_tmpRT, m_cameraColorRT, Settings.CompositeMaterial);

                    context.ExecuteCommandBuffer(cmd);
                }

                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(m_prepassId);
                cmd.ReleaseTemporaryRT(m_blurredId);
                cmd.ReleaseTemporaryRT(m_tmpTexId);
                if (m_prepassHandle != null)
                {
                    m_prepassHandle.Release();
                }
            }
        }

        private RenderSelectionPass m_scriptablePass;

        public override void Create()
        {
            m_scriptablePass = new RenderSelectionPass();
            m_scriptablePass.Settings = m_settings;
            m_scriptablePass.renderPassEvent = m_settings.RenderPassEvent;
        }

#if UNIVERSAL_RP_13_1_OR_NEWER
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if ((renderingData.cameraData.camera.cullingMask & m_settings.LayerMask) != 0)
            {
                renderer.EnqueuePass(m_scriptablePass);
            }
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if ((renderingData.cameraData.camera.cullingMask & m_settings.LayerMask) != 0)
            {
                IMeshesCache meshesCache = IOC.Resolve<IMeshesCache>(m_settings.MeshesCacheName);
                IRenderersCache renderersCache = IOC.Resolve<IRenderersCache>(m_settings.RenderersCacheName);
                ICustomOutlineRenderersCache customRenderersCache = IOC.Resolve<ICustomOutlineRenderersCache>(m_settings.CustomRenderersCacheName);

                if ((meshesCache == null || meshesCache.IsEmpty) && (renderersCache == null || renderersCache.IsEmpty) && (customRenderersCache == null || customRenderersCache.GetOutlineRendererItems().Count == 0))
                {
                    return;
                }

                var src = renderer.cameraColorTargetHandle;
                m_scriptablePass.Setup(src, meshesCache, renderersCache, customRenderersCache);
            }
        }

#else
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if ((renderingData.cameraData.camera.cullingMask & m_settings.LayerMask) != 0)
            {
                IMeshesCache meshesCache = IOC.Resolve<IMeshesCache>(m_settings.MeshesCacheName);
                IRenderersCache renderersCache = IOC.Resolve<IRenderersCache>(m_settings.RenderersCacheName);
                ICustomOutlineRenderersCache customRenderersCache = IOC.Resolve<ICustomOutlineRenderersCache>(m_settings.CustomRenderersCacheName);

                if ((meshesCache == null || meshesCache.IsEmpty) && (renderersCache == null || renderersCache.IsEmpty) && (customRenderersCache == null || customRenderersCache.GetOutlineRendererItems().Count == 0))
                {
                    return;
                }

                var src = renderer.cameraColorTarget;
                m_scriptablePass.Setup(src, meshesCache, renderersCache, customRenderersCache);
                renderer.EnqueuePass(m_scriptablePass);
            }
        }
#endif
    }
}
