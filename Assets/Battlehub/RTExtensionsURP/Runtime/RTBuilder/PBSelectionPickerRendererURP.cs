using System.Collections.Generic;
using System.Linq;
using Battlehub.ProBuilderIntegration;
using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.Universal;
namespace Battlehub.RTBuilder.URP
{
    public class PBSelectionPickerRendererURP : PBSelectionPickerRenderer
    {
        private IRenderersCache m_cache;
        public PBSelectionPickerRendererURP(IRenderersCache cache)
        {
            m_cache = cache;
        }

        protected override void PrepareCamera(Camera renderCamera)
        {
            base.PrepareCamera(renderCamera);

            UniversalAdditionalCameraData cameraData = renderCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.renderShadows = false;

            cameraData.SetRenderer(1);
        }

        protected override void Render(Shader shader, string tag, Camera renderCam)
        {
            bool invertCulling = GL.invertCulling;

            GL.invertCulling = true;
            renderCam.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
            renderCam.Render();
            GL.invertCulling = invertCulling;

            m_cache.Clear();
        }

        protected override void GenerateEdgePickingObjects(IList<ProBuilderMesh> selection, bool doDepthTest, out Dictionary<uint, SimpleTuple<ProBuilderMesh, Edge>> map, out GameObject[] depthObjects, out GameObject[] pickerObjects)
        {
            base.GenerateEdgePickingObjects(selection, doDepthTest, out map, out depthObjects, out pickerObjects);
            if(depthObjects != null)
            {
                m_cache.Add(depthObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
            if(pickerObjects != null)
            {
                m_cache.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
        }

        protected override GameObject[] GenerateFacePickingObjects(IList<ProBuilderMesh> selection, out Dictionary<uint, SimpleTuple<ProBuilderMesh, Face>> map)
        {
            GameObject[] pickerObjects = base.GenerateFacePickingObjects(selection, out map);
            m_cache.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            return pickerObjects;
        }

        protected override void GenerateVertexPickingObjects(IList<ProBuilderMesh> selection, bool doDepthTest, out Dictionary<uint, SimpleTuple<ProBuilderMesh, int>> map, out GameObject[] depthObjects, out GameObject[] pickerObjects)
        {
            base.GenerateVertexPickingObjects(selection, doDepthTest, out map, out depthObjects, out pickerObjects);
            if(depthObjects != null)
            {
                m_cache.Add(depthObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
            if(pickerObjects != null)
            {
                m_cache.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
        }
    }
}
