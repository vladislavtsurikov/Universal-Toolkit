using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
#if BillboardSystem
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData.BillboardSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager
{
    public static class RenderManager 
    {
        private static readonly GPUInstancedIndirectRenderer _gpuInstancedIndirectRenderer = new GPUInstancedIndirectRenderer();
#if BillboardSystem
        public static RenderImposterCells RenderImposterCells = new RenderImposterCells();
#endif
        
        public static void Render(Type rendererType, SelectionData selectionData)
        {
            Profiler.BeginSample("Render");

            if (!_gpuInstancedIndirectRenderer.IsSetup)
            {
                _gpuInstancedIndirectRenderer.Setup();
            }
            
            CameraManager cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            for (int cameraIndex = 0; cameraIndex <= cameraManager.VirtualCameraList.Count - 1; cameraIndex++)
            {            
                if(cameraManager.VirtualCameraList[cameraIndex].Ignored)
                {
                    continue;
                }
                
                Camera targetCamera = cameraManager.VirtualCameraList[cameraIndex].GetRenderingCamera();

                for (int protoIndex = 0; protoIndex <= selectionData.PrototypeList.Count - 1; protoIndex++)
                {
                    Prototype proto = selectionData.PrototypeList[protoIndex];

                    if (proto.Active == false)
                    {
                        continue;
                    }

                    if (Utility.Utility.IsInLayer(cameraManager.VirtualCameraList[cameraIndex].Camera.cullingMask,
                            proto.Layer) == false)
                    {
                        continue;
                    }
                    
                    Shadow shadowSettings = (Shadow)proto.GetSettings(typeof(Shadow));
                    
                    Quality quality = (Quality)GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality),rendererType);

                    ShadowCastingMode shadowCastingMode = quality.GetShadowCastingMode();
                    
                    if (!PrototypeComponent.IsValid(shadowSettings))
                    {
                        shadowCastingMode = ShadowCastingMode.Off;
                    }
                    
                    DistanceCulling distanceCulling = (DistanceCulling)proto.GetSettings(typeof(DistanceCulling));
                    
                    float maxDistance = Utility.Utility.GetMaxDistance(rendererType, cameraManager.VirtualCameraList[cameraIndex], distanceCulling);

#if BillboardSystem
                    RenderImposterCells.Render(prototypesPackage, protoIndex, cameraIndex, targetCamera, shadowCastingMode, maxDistance);
#else
                    _gpuInstancedIndirectRenderer.Render(selectionData, protoIndex, cameraIndex, targetCamera, shadowCastingMode, maxDistance);
#endif
                }
            }
            
            Profiler.EndSample();
        }
    }
}