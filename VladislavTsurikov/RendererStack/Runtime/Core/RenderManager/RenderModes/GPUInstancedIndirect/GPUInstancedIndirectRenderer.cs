using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect
{
    public class GPUInstancedIndirectRenderer
    {
        [NonSerialized]
        public bool IsSetup;

        private int _gpuInstancedIndirectDataBufferID;

        public void Setup()
        {
            _gpuInstancedIndirectDataBufferID = Shader.PropertyToID("GPUInstancedIndirectDataBuffer");
            IsSetup = true;
        }

        public void Render(SelectionData selectionData, int protoIndex, int cameraIndex, Camera targetCamera, ShadowCastingMode shadowCastingMode, float maxDistance)
        {
            CameraManager cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));
            
            VirtualCamera virtualCamera = cameraManager.VirtualCameraList[cameraIndex];
            ObjectCameraRender objectCameraRender = (ObjectCameraRender)virtualCamera.CameraTemporaryComponentStack.GetElement(typeof(ObjectCameraRender));

            Prototype proto = selectionData.PrototypeList[protoIndex];
            
            RenderModel renderModel = proto.RenderModel;

            float boundsDistance = maxDistance * 2 + renderModel.BoundingSphereRadius;
            Bounds cellBounds = new Bounds(virtualCamera.Camera.transform.position, new Vector3(boundsDistance, boundsDistance, boundsDistance));
            
            LayerMask layer = proto.Layer;

            for (int lodIndex = 0; lodIndex < renderModel.LODs.Count; lodIndex++)
            {
                var lod = renderModel.LODs[lodIndex];

                lod.MaterialPropertyBlock.Clear();
                ComputeBuffer visibleBuffer = objectCameraRender.GetLODVisibleBuffer(protoIndex, lodIndex, false);
                lod.MaterialPropertyBlock.SetBuffer(_gpuInstancedIndirectDataBufferID, visibleBuffer);
                
                if (shadowCastingMode == ShadowCastingMode.On)
                {
                    lod.ShadowMaterialPropertyBlock.Clear();
                    ComputeBuffer shadowVisibleBuffer = objectCameraRender.GetLODVisibleBuffer(protoIndex, lodIndex, true);
                    lod.ShadowMaterialPropertyBlock.SetBuffer(_gpuInstancedIndirectDataBufferID, shadowVisibleBuffer);
                }

                for (int m = 0; m < lod.Materials.Count; m++)
                {
                    var material = lod.Materials[m];

                    var submeshIndex = Mathf.Min(m, lod.Mesh.subMeshCount - 1);

                    List<ComputeBufferProperty> argsBufferList = objectCameraRender.GetLODArgsBufferList(protoIndex, lodIndex, false);

                    Graphics.DrawMeshInstancedIndirect(lod.Mesh, submeshIndex,
                        material,
                        cellBounds,
                        argsBufferList[submeshIndex].ComputeBuffer,
                        0,
                        lod.MaterialPropertyBlock,
                        ShadowCastingMode.Off, true, layer,
                        targetCamera
#if UNITY_2018_1_OR_NEWER
                        , LightProbeUsage.Off
#endif
                        );

                    if (shadowCastingMode == ShadowCastingMode.On)
                    {
                        List<ComputeBufferProperty> shadowArgsBufferList = objectCameraRender.GetLODArgsBufferList(protoIndex, lodIndex, true);
                        
                        Graphics.DrawMeshInstancedIndirect(lod.Mesh, submeshIndex,
                            material,
                            cellBounds,
                            shadowArgsBufferList[submeshIndex].ComputeBuffer,
                            0,
                            lod.ShadowMaterialPropertyBlock,
                            ShadowCastingMode.ShadowsOnly, true, layer,
                            targetCamera
#if UNITY_2018_1_OR_NEWER
                            , LightProbeUsage.Off
#endif
                        );
                    }
                }
            }
        }
    }
}