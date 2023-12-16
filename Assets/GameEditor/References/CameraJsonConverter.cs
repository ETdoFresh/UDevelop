using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraJsonConverter : ComponentJsonConverter<Camera>
{
    public override void WriteJson(JsonWriter writer, Camera value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        WriteType(writer, value);
        
        var ignoreDefaultValues = serializer.DefaultValueHandling == DefaultValueHandling.Ignore;
        if (!ignoreDefaultValues || value.nearClipPlane != 0.3f)
        {
            writer.WritePropertyName("nearClipPlane");
            writer.WriteValue(value.nearClipPlane);
        }
        if (!ignoreDefaultValues || value.farClipPlane != 1000f)
        {
            writer.WritePropertyName("farClipPlane");
            writer.WriteValue(value.farClipPlane);
        }
        if (!ignoreDefaultValues || value.fieldOfView != 60f)
        {
            writer.WritePropertyName("fieldOfView");
            writer.WriteValue(value.fieldOfView);
        }
        if (!ignoreDefaultValues || value.renderingPath != RenderingPath.UsePlayerSettings)
        {
            writer.WritePropertyName("renderingPath");
            writer.WriteValue(value.renderingPath);
        }
        if (!ignoreDefaultValues || value.allowHDR != true)
        {
            writer.WritePropertyName("allowHDR");
            writer.WriteValue(value.allowHDR);
        }
        if (!ignoreDefaultValues || value.allowMSAA != true)
        {
            writer.WritePropertyName("allowMSAA");
            writer.WriteValue(value.allowMSAA);
        }
        if (!ignoreDefaultValues || value.allowDynamicResolution != false)
        {
            writer.WritePropertyName("allowDynamicResolution");
            writer.WriteValue(value.allowDynamicResolution);
        }
        if (!ignoreDefaultValues || value.forceIntoRenderTexture != false)
        {
            writer.WritePropertyName("forceIntoRenderTexture");
            writer.WriteValue(value.forceIntoRenderTexture);
        }
        if (!ignoreDefaultValues || value.orthographicSize != 5f)
        {
            writer.WritePropertyName("orthographicSize");
            writer.WriteValue(value.orthographicSize);
        }
        if (!ignoreDefaultValues || value.orthographic != false)
        {
            writer.WritePropertyName("orthographic");
            writer.WriteValue(value.orthographic);
        }
        if (!ignoreDefaultValues || value.opaqueSortMode != OpaqueSortMode.Default)
        {
            writer.WritePropertyName("opaqueSortMode");
            writer.WriteValue(value.opaqueSortMode);
        }
        if (!ignoreDefaultValues || value.transparencySortMode != TransparencySortMode.Default)
        {
            writer.WritePropertyName("transparencySortMode");
            writer.WriteValue(value.transparencySortMode);
        }
        if (!ignoreDefaultValues || value.depth != -1)
        {
            writer.WritePropertyName("depth");
            writer.WriteValue(value.depth);
        }
        if (!ignoreDefaultValues || value.cullingMask != -1)
        {
            writer.WritePropertyName("cullingMask");
            writer.WriteValue(value.cullingMask);
        }
        if (!ignoreDefaultValues || value.eventMask != -1)
        {
            writer.WritePropertyName("eventMask");
            writer.WriteValue(value.eventMask);
        }
        if (!ignoreDefaultValues || value.layerCullSpherical != false)
        {
            writer.WritePropertyName("layerCullSpherical");
            writer.WriteValue(value.layerCullSpherical);
        }
        if (!ignoreDefaultValues || value.cameraType != CameraType.Game)
        {
            writer.WritePropertyName("cameraType");
            writer.WriteValue(value.cameraType);
        }
        if (!ignoreDefaultValues || value.overrideSceneCullingMask != 0)
        {
            writer.WritePropertyName("overrideSceneCullingMask");
            writer.WriteValue(value.overrideSceneCullingMask);
        }
        if (!ignoreDefaultValues || value.backgroundColor != new Color(0.1921569f, 0.3019608f, 0.4745098f, 0f))
        {
            writer.WritePropertyName("backgroundColor");
            writer.WriteValue(value.backgroundColor);
        }
        if (!ignoreDefaultValues || value.clearFlags != CameraClearFlags.Skybox)
        {
            writer.WritePropertyName("clearFlags");
            writer.WriteValue(value.clearFlags);
        }
        if (!ignoreDefaultValues || value.depthTextureMode != DepthTextureMode.None)
        {
            writer.WritePropertyName("depthTextureMode");
            writer.WriteValue(value.depthTextureMode);
        }
        if (!ignoreDefaultValues || value.clearStencilAfterLightingPass != false)
        {
            writer.WritePropertyName("clearStencilAfterLightingPass");
            writer.WriteValue(value.clearStencilAfterLightingPass);
        }
        if (!ignoreDefaultValues || value.usePhysicalProperties != false)
        {
            writer.WritePropertyName("usePhysicalProperties");
            writer.WriteValue(value.usePhysicalProperties);
        }
        if (!ignoreDefaultValues || value.iso != 200)
        {
            writer.WritePropertyName("iso");
            writer.WriteValue(value.iso);
        }
        if (!ignoreDefaultValues || value.shutterSpeed != 0.005f)
        {
            writer.WritePropertyName("shutterSpeed");
            writer.WriteValue(value.shutterSpeed);
        }
        if (!ignoreDefaultValues || value.aperture != 16f)
        {
            writer.WritePropertyName("aperture");
            writer.WriteValue(value.aperture);
        }
        if (!ignoreDefaultValues || value.focusDistance != 10f)
        {
            writer.WritePropertyName("focusDistance");
            writer.WriteValue(value.focusDistance);
        }
        if (!ignoreDefaultValues || value.focalLength != 50f)
        {
            writer.WritePropertyName("focalLength");
            writer.WriteValue(value.focalLength);
        }
        if (!ignoreDefaultValues || value.bladeCount != 5)
        {
            writer.WritePropertyName("bladeCount");
            writer.WriteValue(value.bladeCount);
        }
        if (!ignoreDefaultValues || value.barrelClipping != 0.25f)
        {
            writer.WritePropertyName("barrelClipping");
            writer.WriteValue(value.barrelClipping);
        }
        if (!ignoreDefaultValues || value.anamorphism != 0f)
        {
            writer.WritePropertyName("anamorphism");
            writer.WriteValue(value.anamorphism);
        }
        if (!ignoreDefaultValues || value.sensorSize != new Vector2(36f, 24f))
        {
            writer.WritePropertyName("sensorSize");
            writer.WriteValue(value.sensorSize);
        }
        if (!ignoreDefaultValues || value.lensShift != new Vector2(0f, 0f))
        {
            writer.WritePropertyName("lensShift");
            writer.WriteValue(value.lensShift);
        }
        if (!ignoreDefaultValues || value.gateFit != Camera.GateFitMode.Horizontal)
        {
            writer.WritePropertyName("gateFit");
            writer.WriteValue(value.gateFit);
        }
        if (!ignoreDefaultValues || value.rect != new Rect(0f, 0f, 1f, 1f))
        {
            writer.WritePropertyName("rect");
            writer.WriteValue(value.rect);
        }
        if (!ignoreDefaultValues || value.targetTexture != null)
        {
            writer.WritePropertyName("targetTexture");
            serializer.Serialize(writer, value.targetTexture);
        }
        if (!ignoreDefaultValues || value.targetDisplay != 0)
        {
            writer.WritePropertyName("targetDisplay");
            writer.WriteValue(value.targetDisplay);
        }
        if (!ignoreDefaultValues || value.useJitteredProjectionMatrixForTransparentRendering != true)
        {
            writer.WritePropertyName("useJitteredProjectionMatrixForTransparentRendering");
            writer.WriteValue(value.useJitteredProjectionMatrixForTransparentRendering);
        }
        if (!ignoreDefaultValues || value.stereoSeparation != 0.022f)
        {
            writer.WritePropertyName("stereoSeparation");
            writer.WriteValue(value.stereoSeparation);
        }
        if (!ignoreDefaultValues || value.stereoConvergence != 10f)
        {
            writer.WritePropertyName("stereoConvergence");
            writer.WriteValue(value.stereoConvergence);
        }
        if (!ignoreDefaultValues || value.stereoTargetEye != StereoTargetEyeMask.Both)
        {
            writer.WritePropertyName("stereoTargetEye");
            writer.WriteValue(value.stereoTargetEye);
        }
        writer.WriteEndObject();
    }
}