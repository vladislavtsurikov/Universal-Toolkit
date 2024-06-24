using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender
{
    public class ObjectCameraRender : CameraTemporaryComponent
    {
        public readonly List<PrototypeRenderData> PrototypeRenderDataList = new List<PrototypeRenderData>();

        protected override void SetupCameraTemporaryComponent()
        {
            PrototypeRenderer prototypeRenderer = RendererStackManager.Instance.RendererStack.GetElement(typeof(TerrainObjectRenderer.TerrainObjectRenderer)) as PrototypeRenderer;

            PrototypeRenderDataList.Clear();

            if (prototypeRenderer == null)
            {
                return;
            }

            foreach (Prototype proto in prototypeRenderer.SelectionData.PrototypeList)
            {
                PrototypeRenderData prototypeRenderData = new PrototypeRenderData(proto.RenderModel.LODs); 
                PrototypeRenderDataList.Add(prototypeRenderData);
            }
        }

        protected override void OnCreate()
        {
            TerrainObjectRenderer.TerrainObjectRenderer instance = (TerrainObjectRenderer.TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.GetElement(typeof(TerrainObjectRenderer.TerrainObjectRenderer));

            if (instance != null)
            {
                instance.ForceUpdateRendererData = true;
            }
        }

        protected override void OnDisableElement()
        {
            foreach (var prototypeRender in PrototypeRenderDataList)
            {
                prototypeRender.Dispose();    
            }
        }

        public ComputeBuffer GetLODVisibleBuffer(int protoIndex, int lodIndex, bool shadows)
        {
            if (shadows)
            {
                return PrototypeRenderDataList[protoIndex].LODRenderDataList[lodIndex].PositionShadowBuffer.ComputeBuffer;
            }
            else
            {
                return PrototypeRenderDataList[protoIndex].LODRenderDataList[lodIndex].PositionBuffer.ComputeBuffer;
            }                      
        }

        public List<ComputeBufferProperty> GetLODArgsBufferList(int protoIndex, int lodIndex, bool shadows)
        {
            if (shadows)
            {
                return PrototypeRenderDataList[protoIndex].LODRenderDataList[lodIndex].ShadowArgsBufferMergedLODList;
            }
            else
            {
                return PrototypeRenderDataList[protoIndex].LODRenderDataList[lodIndex].ArgsBufferMergedLODList;
            }
        }
    }
}