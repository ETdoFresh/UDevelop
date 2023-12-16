using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UniversalAdditionalCameraDataJsonConverter : ComponentJsonConverter<UniversalAdditionalCameraData>
{
    public override void WriteJson(JsonWriter writer, UniversalAdditionalCameraData value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        WriteType(writer, value);
        
        var ignoreDefaultValues = serializer.DefaultValueHandling == DefaultValueHandling.Ignore;
        if (!ignoreDefaultValues || value.renderShadows != true)
        {
            writer.WritePropertyName("m_RenderShadows");
            writer.WriteValue(value.renderShadows);
        }
        if (!ignoreDefaultValues || value.requiresDepthOption != CameraOverrideOption.UsePipelineSettings)
        {
            writer.WritePropertyName("m_RequiresDepthTextureOption");
            writer.WriteValue(value.requiresDepthOption);
        }
        if (!ignoreDefaultValues || value.requiresColorOption != CameraOverrideOption.UsePipelineSettings)
        {
            writer.WritePropertyName("m_RequiresOpaqueTextureOption");
            writer.WriteValue(value.requiresColorOption);
        }
        if (!ignoreDefaultValues || value.renderType != CameraRenderType.Base)
        {
            writer.WritePropertyName("m_CameraType");
            writer.WriteValue(value.renderType);
        }
        if (!ignoreDefaultValues || value.cameraStack != null && value.cameraStack.Count > 0)
        {
            writer.WritePropertyName("m_Cameras");
            writer.WriteStartArray();
            foreach (var camera in value.cameraStack)
            {
                serializer.Serialize(writer, camera);
            }
            writer.WriteEndArray();
        }
        // if (!ignoreDefaultValues || value.rendererIndex != -1)
        // {
        //     writer.WritePropertyName("m_RendererIndex");
        //     writer.WriteValue(value.rendererIndex);
        // }
        if (!ignoreDefaultValues || value.volumeLayerMask != 1)
        {
            writer.WritePropertyName("m_VolumeLayerMask");
            serializer.Serialize(writer, value.volumeLayerMask);
        }
        if (!ignoreDefaultValues || value.volumeTrigger != null)
        {
            writer.WritePropertyName("m_VolumeTrigger");
            serializer.Serialize(writer, value.volumeTrigger);
        }
        // if (!ignoreDefaultValues || value.volumeFrameworkUpdateMode != VolumeFrameworkUpdateMode.Automatic)
        // {
        //     writer.WritePropertyName("m_VolumeFrameworkUpdateModeOption");
        //     writer.WriteValue(value.volumeFrameworkUpdateMode);
        // }
        if (!ignoreDefaultValues || value.renderPostProcessing != false)
        {
            writer.WritePropertyName("m_RenderPostProcessing");
            writer.WriteValue(value.renderPostProcessing);
        }
        if (!ignoreDefaultValues || value.antialiasing != AntialiasingMode.None)
        {
            writer.WritePropertyName("m_Antialiasing");
            writer.WriteValue(value.antialiasing);
        }
        if (!ignoreDefaultValues || value.antialiasingQuality != AntialiasingQuality.High)
        {
            writer.WritePropertyName("m_AntialiasingQuality");
            writer.WriteValue(value.antialiasingQuality);
        }
        if (!ignoreDefaultValues || value.stopNaN != false)
        {
            writer.WritePropertyName("m_StopNaN");
            writer.WriteValue(value.stopNaN);
        }
        if (!ignoreDefaultValues || value.dithering != false)
        {
            writer.WritePropertyName("m_Dithering");
            writer.WriteValue(value.dithering);
        }
        if (!ignoreDefaultValues || value.clearDepth != true)
        {
            writer.WritePropertyName("m_ClearDepth");
            writer.WriteValue(value.clearDepth);
        }
        if (!ignoreDefaultValues || value.allowXRRendering != true)
        {
            writer.WritePropertyName("m_AllowXRRendering");
            writer.WriteValue(value.allowXRRendering);
        }
        if (!ignoreDefaultValues || value.allowHDROutput != true)
        {
            writer.WritePropertyName("m_AllowHDROutput");
            writer.WriteValue(value.allowHDROutput);
        }
        if (!ignoreDefaultValues || value.useScreenCoordOverride != false)
        {
            writer.WritePropertyName("m_UseScreenCoordOverride");
            writer.WriteValue(value.useScreenCoordOverride);
        }
        if (!ignoreDefaultValues || value.screenSizeOverride != new Vector4(0f, 0f, 0f, 0f))
        {
            writer.WritePropertyName("m_ScreenSizeOverride");
            writer.WriteValue(value.screenSizeOverride);
        }
        if (!ignoreDefaultValues || value.screenCoordScaleBias != new Vector4(0f, 0f, 0f, 0f))
        {
            writer.WritePropertyName("m_ScreenCoordScaleBias");
            writer.WriteValue(value.screenCoordScaleBias);
        }
        if (!ignoreDefaultValues || value.requiresDepthTexture != false)
        {
            writer.WritePropertyName("m_RequiresDepthTexture");
            writer.WriteValue(value.requiresDepthTexture);
        }
        if (!ignoreDefaultValues || value.requiresColorTexture != false)
        {
            writer.WritePropertyName("m_RequiresColorTexture");
            writer.WriteValue(value.requiresColorTexture);
        }
        if (!ignoreDefaultValues || value.version != 2)
        {
            writer.WritePropertyName("m_Version");
            writer.WriteValue(value.version);
        }
        // if (!ignoreDefaultValues || value.taaSettings != null)
        // {
        //     writer.WritePropertyName("m_TaaSettings");
        //     serializer.Serialize(writer, value.taaSettings);
        // }
        writer.WriteEndObject();
    }
}