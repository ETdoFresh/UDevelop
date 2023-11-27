﻿using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Battlehub.RTCommon
{
    public class ExecuteCommandBuffers : ScriptableRendererFeature
    {
        private class RendererPass : ScriptableRenderPass
        {
            private CameraEvent m_cameraEvent;

            public RendererPass(CameraEvent cameraEvent)
            {
                m_cameraEvent = cameraEvent;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.camera.commandBufferCount > 0)
                {
                    CommandBuffer[] cmdBuffer = renderingData.cameraData.camera.GetCommandBuffers(m_cameraEvent);
                    for(int i = 0; i < cmdBuffer.Length; ++i)
                    {
                        context.ExecuteCommandBuffer(cmdBuffer[i]);
                    }
                }
            }
        }

        private RendererPass[] m_scriptablePasses;

        public override void Create()
        {
            m_scriptablePasses = new[]
            {
                CreatePass(CameraEvent.AfterForwardAlpha),
                CreatePass(CameraEvent.BeforeImageEffects),
                CreatePass(CameraEvent.AfterImageEffectsOpaque),
                CreatePass(CameraEvent.AfterImageEffects),
            };
        }

        private RendererPass CreatePass(CameraEvent camEvent)
        {
            return new RendererPass(camEvent) { renderPassEvent = ToRenderPassEvent(camEvent) };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            for(int i = 0; i < m_scriptablePasses.Length; ++i)
            {
                renderer.EnqueuePass(m_scriptablePasses[i]);
            }  
        }

        public RenderPassEvent ToRenderPassEvent(CameraEvent cameraEvent)
        {
            switch(cameraEvent)
            {
                case CameraEvent.BeforeImageEffects:
                    return RenderPassEvent.BeforeRenderingPostProcessing;
                case CameraEvent.AfterImageEffects:
                case CameraEvent.AfterImageEffectsOpaque:
                    return RenderPassEvent.AfterRenderingPostProcessing;
                case CameraEvent.AfterForwardAlpha:
                    return RenderPassEvent.AfterRenderingTransparents;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}

