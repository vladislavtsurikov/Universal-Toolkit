using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime.UnmanagedProperties;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem.Components.ObjectCameraRender;
using VladislavTsurikov.RendererStack.Runtime.Core.Utility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Components.Camera;
using VladislavTsurikov.Utility.Runtime;
using LODGroup = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings.Components.LODGroup;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GPUInstancedIndirect.ComputeShaderKernelProperties
{
    public static class GPUFrustumCullingID 
    {
        private struct BatchAdd
        {
            public ComputeBuffer VisibleBuffer;
            public ComputeBuffer ShadowBuffer;
        }

        private static readonly ComputeBufferProperty _dummyComputeBuffer = new ComputeBufferProperty();
        private static int _frustumKernelHandle;
        private static ComputeShader _frustumCulling;
        private static int _cameraFrustumPlan0;
        private static int _cameraFrustumPlan1;
        private static int _cameraFrustumPlan2;
        private static int _cameraFrustumPlan3;
        private static int _cameraFrustumPlan4;
        private static int _cameraFrustumPlan5;
        private static int _fieldOfViewID = -1;

        private static int _floatingOriginOffsetID = -1;
        private static int _worldSpaceCameraPosID = -1;
        public static int MergeBufferID = -1;

        private static readonly int[] _positionLod = new int[] {-1, -1, -1, -1};
        private static readonly int[] _shadowPositionLod = new int[] {-1, -1, -1, -1};

        private static int _positionsID = -1;
        private static int _instanceCountID = -1;

        private static int _boundingSphereRadiusID = -1;
        private static int _isFrustumCullingID = -1;
        private static int _isDistanceCullingID = -1;

        private static int _lodFadeDistanceID = -1;
        private static int _lodFadeForLastLodID = -1;
        private static int _lodDistanceRandomOffsetID = -1;
        private static int _isStandardRenderPipelineID = -1;

        private static int _useLODFadeID = -1;
        private static int _getAdditionalShadowID = -1;
        private static int _shadowDistanceID = -1;
        private static int _minCullingDistanceID = -1;
        private static int _increaseBoundingSphereForShadowsID = -1;

        private static int _lodDistancesID = -1;
        private static int _shadowLODMapID = -1;
        private static int _lodCountID = -1;
        private static int _startLOD = -1;
        private static int _maxDistanceID = -1;
        private static int _minDistanceID = -1;
        private static int _distanceRandomOffsetID = -1;

        private static int _directionLightID = -1;
        private static int _boundsSizeID = -1;

        public static readonly float Numthreads = 512f;

        public static void Setup()
        {
            _dummyComputeBuffer.ChangeComputeBuffer(new ComputeBuffer(1, 16 * 4 * 2 + 16, ComputeBufferType.Append));
            _dummyComputeBuffer.ComputeBuffer.SetCounterValue(0);

            _frustumCulling = (ComputeShader)Resources.Load("RendererStackGPUFrustumCullingLOD");
            _frustumKernelHandle = _frustumCulling.FindKernel("RendererStackGPUFrustumCullingLOD"); 
            MergeBufferID = Shader.PropertyToID("MergeBuffer");

            _floatingOriginOffsetID = Shader.PropertyToID("floatingOriginOffset");

            _worldSpaceCameraPosID = Shader.PropertyToID("worldSpaceCameraPos");

            _cameraFrustumPlan0 = Shader.PropertyToID("cameraFrustumPlane0");
            _cameraFrustumPlan1 = Shader.PropertyToID("cameraFrustumPlane1");
            _cameraFrustumPlan2 = Shader.PropertyToID("cameraFrustumPlane2");
            _cameraFrustumPlan3 = Shader.PropertyToID("cameraFrustumPlane3");
            _cameraFrustumPlan4 = Shader.PropertyToID("cameraFrustumPlane4");
            _cameraFrustumPlan5 = Shader.PropertyToID("cameraFrustumPlane5");
            _fieldOfViewID = Shader.PropertyToID("fieldOfView");

            _instanceCountID = Shader.PropertyToID("instanceCount");
            _positionsID = Shader.PropertyToID("positions");
            _positionLod[0] = Shader.PropertyToID("positionLOD0");
            _positionLod[1] = Shader.PropertyToID("positionLOD1");
            _positionLod[2] = Shader.PropertyToID("positionLOD2");
            _positionLod[3] = Shader.PropertyToID("positionLOD3");
            
            _shadowPositionLod[0] = Shader.PropertyToID("positionShadowLOD0");
            _shadowPositionLod[1] = Shader.PropertyToID("positionShadowLOD1");
            _shadowPositionLod[2] = Shader.PropertyToID("positionShadowLOD2");
            _shadowPositionLod[3] = Shader.PropertyToID("positionShadowLOD3");
            
            _isFrustumCullingID = Shader.PropertyToID("isFrustumCulling");
            _isDistanceCullingID = Shader.PropertyToID("isDistanceCulling");
            
            _boundingSphereRadiusID = Shader.PropertyToID("boundingSphereRadius");

            _useLODFadeID = Shader.PropertyToID("useLODFade");
            _getAdditionalShadowID = Shader.PropertyToID("getAdditionalShadow");
            _shadowDistanceID = Shader.PropertyToID("shadowDistance");
            _minCullingDistanceID = Shader.PropertyToID("minCullingDistance");
            _increaseBoundingSphereForShadowsID = Shader.PropertyToID("increaseBoundingSphereForShadows");

            _lodFadeDistanceID = Shader.PropertyToID("LODFadeDistance");
            _lodFadeForLastLodID = Shader.PropertyToID("lodFadeForLastLOD");
            _lodDistanceRandomOffsetID = Shader.PropertyToID("LODDistanceRandomOffset");
            _isStandardRenderPipelineID = Shader.PropertyToID("isStandardRenderPipeline");
            

            _lodDistancesID = Shader.PropertyToID("lodDistances");
            _shadowLODMapID = Shader.PropertyToID("shadowLODMap");
            _lodCountID = Shader.PropertyToID("LODCount");
            _startLOD = Shader.PropertyToID("startLOD");
            _maxDistanceID = Shader.PropertyToID("maxDistance");
            _minDistanceID = Shader.PropertyToID("minDistance");
            _distanceRandomOffsetID = Shader.PropertyToID("distanceRandomOffset");

            _directionLightID = Shader.PropertyToID("directionLight");
            _boundsSizeID = Shader.PropertyToID("boundsSize");
        }

        public static void SetFrustumCullingPlanes(Camera selectedCamera)
        {
            GeometryUtilityAllocFree.CalculateFrustumPlanes(selectedCamera);

            Vector4 cameraFrustumPlane0 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[0].normal.x, GeometryUtilityAllocFree.FrustumPlanes[0].normal.y, GeometryUtilityAllocFree.FrustumPlanes[0].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[0].distance);
            Vector4 cameraFrustumPlane1 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[1].normal.x, GeometryUtilityAllocFree.FrustumPlanes[1].normal.y, GeometryUtilityAllocFree.FrustumPlanes[1].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[1].distance);
            Vector4 cameraFrustumPlane2 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[2].normal.x, GeometryUtilityAllocFree.FrustumPlanes[2].normal.y, GeometryUtilityAllocFree.FrustumPlanes[2].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[2].distance);
            Vector4 cameraFrustumPlane3 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[3].normal.x, GeometryUtilityAllocFree.FrustumPlanes[3].normal.y, GeometryUtilityAllocFree.FrustumPlanes[3].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[3].distance);
            Vector4 cameraFrustumPlane4 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[4].normal.x, GeometryUtilityAllocFree.FrustumPlanes[4].normal.y, GeometryUtilityAllocFree.FrustumPlanes[4].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[4].distance);
            Vector4 cameraFrustumPlane5 = new Vector4(GeometryUtilityAllocFree.FrustumPlanes[5].normal.x, GeometryUtilityAllocFree.FrustumPlanes[5].normal.y, GeometryUtilityAllocFree.FrustumPlanes[5].normal.z,
                GeometryUtilityAllocFree.FrustumPlanes[5].distance);

            _frustumCulling.SetVector(_cameraFrustumPlan0, cameraFrustumPlane0);
            _frustumCulling.SetVector(_cameraFrustumPlan1, cameraFrustumPlane1);
            _frustumCulling.SetVector(_cameraFrustumPlan2, cameraFrustumPlane2);
            _frustumCulling.SetVector(_cameraFrustumPlan3, cameraFrustumPlane3);
            _frustumCulling.SetVector(_cameraFrustumPlan4, cameraFrustumPlane4); 
            _frustumCulling.SetVector(_cameraFrustumPlan5, cameraFrustumPlane5);
            _frustumCulling.SetFloat(_fieldOfViewID, selectedCamera.fieldOfView);

            Vector3 worldSpaceCameraPosition = selectedCamera.transform.position;
            Vector4 worldSpaceCameraPos = new Vector4(worldSpaceCameraPosition.x, worldSpaceCameraPosition.y, worldSpaceCameraPosition.z, 1);
            _frustumCulling.SetVector(_worldSpaceCameraPosID, worldSpaceCameraPos);
        }

        public static void SetComputeShaderGeneralParams(RendererStackManager instantRenderer, PrototypeRenderData prototypeRenderData, PrototypeTerrainObject proto, RenderModel renderModel, VirtualCamera virtualCamera, int totalInstanceCount, float lodBias)
        {
            TerrainObjectRendererCameraSettings terrainObjectRendererCameraSettings = (TerrainObjectRendererCameraSettings)virtualCamera.CameraComponentStack.GetElement(typeof(TerrainObjectRendererCameraSettings));
            
            Quality quality = (Quality)instantRenderer.SceneComponentStack.GetElement(typeof(Quality));

            FrustumCulling frustumCulling = (FrustumCulling)proto.GetSettings(typeof(FrustumCulling));
            LODGroup lodGroup = (LODGroup)proto.GetSettings(typeof(LODGroup));
            DistanceCulling distanceCulling = (DistanceCulling)proto.GetSettings(typeof(DistanceCulling));
            Shadow shadow = (Shadow)proto.GetSettings(typeof(Shadow));

            Vector3 directionLight = quality.DirectionalLight ? quality.DirectionalLight.transform.forward : new Vector3(0, 0, 0);

            float maxDistance = Core.Utility.Utility.GetMaxDistance(typeof(TerrainObjectRenderer), virtualCamera, distanceCulling);

            Vector4 floatingOriginOffsetVector4 = new Vector4(quality.FloatingOriginOffset.x, quality.FloatingOriginOffset.y, quality.FloatingOriginOffset.z, 0);

            _frustumCulling.SetVector(_floatingOriginOffsetID, floatingOriginOffsetVector4);

            _frustumCulling.SetBuffer(_frustumKernelHandle, _positionsID, prototypeRenderData.MergeBuffer.ComputeBuffer); 

            _frustumCulling.SetInt(_instanceCountID, totalInstanceCount);

            if (PrototypeComponent.IsValid(frustumCulling))
            {
                _frustumCulling.SetBool(_isFrustumCullingID, terrainObjectRendererCameraSettings.CameraCullingMode == CameraCullingMode.FrustumCulling);
                _frustumCulling.SetFloat(_boundingSphereRadiusID, renderModel.BoundingSphereRadius + frustumCulling.IncreaseBoundingSphere);
                _frustumCulling.SetInt(_getAdditionalShadowID, (int)frustumCulling.GetAdditionalShadow);
                _frustumCulling.SetFloat(_minCullingDistanceID, frustumCulling.MinCullingDistance);
                _frustumCulling.SetFloat(_increaseBoundingSphereForShadowsID, frustumCulling.IncreaseShadowsBoundingSphere);
            }
            else
            {
                _frustumCulling.SetBool(_isFrustumCullingID, false);
            }

            _frustumCulling.SetBool(_isDistanceCullingID, PrototypeComponent.IsValid(distanceCulling));

            _frustumCulling.SetBool(_useLODFadeID, lodGroup.EnabledLODFade);

            _frustumCulling.SetFloat(_shadowDistanceID, PrototypeComponent.IsValid(shadow) ? shadow.ShadowDistance : 0);
            
            _frustumCulling.SetFloat(_lodFadeDistanceID, lodGroup.LodFadeTransitionDistance);
            _frustumCulling.SetBool(_lodFadeForLastLodID, lodGroup.LodFadeForLastLOD);
            _frustumCulling.SetFloat(_lodDistanceRandomOffsetID, lodGroup.LODDistanceRandomOffset);
            _frustumCulling.SetBool(_isStandardRenderPipelineID, FindRenderPipeline.IsStandardRP);
            
            float[] lodDistances = Core.Utility.Utility.GetLODDistances(renderModel, lodBias, maxDistance);

            _frustumCulling.SetFloats(_lodDistancesID, lodDistances);
            _frustumCulling.SetFloats(_shadowLODMapID, shadow.ShadowLODMap);
            _frustumCulling.SetInt(_lodCountID, renderModel.LODs.Count);
            _frustumCulling.SetFloat(_maxDistanceID, maxDistance);
            _frustumCulling.SetFloat(_distanceRandomOffsetID, distanceCulling.DistanceRandomOffset);

            _frustumCulling.SetVector(_directionLightID, directionLight);
            _frustumCulling.SetVector(_boundsSizeID, proto.Bounds.size);
        }

        public static void DispatchGPUFrustumCulling(RendererStackManager rendererStackManager, PrototypeRenderData prototypeRenderData, PrototypeTerrainObject proto, RenderModel renderModel, VirtualCamera camera, int totalInstanceCount, int threadGroups)
        {
            TerrainObjectRendererCameraSettings terrainObjectRendererCameraSettings = (TerrainObjectRendererCameraSettings)camera.CameraComponentStack.GetElement(typeof(TerrainObjectRendererCameraSettings));
            Common.GlobalSettings.Components.Quality quality = (Common.GlobalSettings.Components.Quality)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Common.GlobalSettings.Components.Quality), typeof(TerrainObjectRenderer));

            LODGroup lodGroup = (LODGroup)proto.GetSettings(typeof(LODGroup));
            float lodBias = QualitySettings.lodBias * quality.LODBias * lodGroup.LODBias * terrainObjectRendererCameraSettings.LodBias;

            SetComputeShaderGeneralParams(rendererStackManager, prototypeRenderData, proto, renderModel, camera, totalInstanceCount, lodBias);

            List<BatchAdd> batches = new List<BatchAdd>();
            int countDispatch = 0;

            for (int lodIndex = 0; lodIndex < renderModel.LODs.Count; lodIndex++)
            {
                BatchAdd batch = new BatchAdd()
                {
                    VisibleBuffer = prototypeRenderData.LODRenderDataList[lodIndex].PositionBuffer.ComputeBuffer,
                    ShadowBuffer = prototypeRenderData.LODRenderDataList[lodIndex].PositionShadowBuffer.ComputeBuffer,
                };

                batches.Add(batch);

                if(renderModel.LODs.Count < 4)
                {
                    if(renderModel.LODs.Count - 1 == lodIndex)
                    {
                        Dispatch(batches, renderModel, prototypeRenderData, lodGroup, lodBias, threadGroups, countDispatch);
                        batches.Clear();
                        countDispatch++;
                    }
                }
                else if(batches.Count == 4)
                {
                    Dispatch(batches, renderModel, prototypeRenderData, lodGroup, lodBias, threadGroups, countDispatch);
                    batches.Clear();
                    countDispatch++;
                }
            }

            if(batches.Count != 0)
            {
                Dispatch(batches, renderModel, prototypeRenderData, lodGroup, lodBias, threadGroups, countDispatch);
                batches.Clear();
            }
        }

        private static void Dispatch(List<BatchAdd> batches, RenderModel renderModel, PrototypeRenderData prototypeRenderData, LODGroup lodGroup, float lodBias, int threadGroups, int countDispatch)
        {
            for (int lodIndex = 0; lodIndex < batches.Count; lodIndex++)
            {
                _frustumCulling.SetBuffer(_frustumKernelHandle, _positionLod[lodIndex], prototypeRenderData.LODRenderDataList[countDispatch == 0 ? lodIndex : lodIndex + 4].PositionBuffer.ComputeBuffer);
                _frustumCulling.SetBuffer(_frustumKernelHandle, _shadowPositionLod[lodIndex], prototypeRenderData.LODRenderDataList[countDispatch == 0 ? lodIndex : lodIndex + 4].PositionShadowBuffer.ComputeBuffer);
            }
            
            for (int i = batches.Count; i < 4; i++)
            {
                _frustumCulling.SetBuffer(_frustumKernelHandle, _positionLod[i], _dummyComputeBuffer.ComputeBuffer);
                _frustumCulling.SetBuffer(_frustumKernelHandle, _shadowPositionLod[i], _dummyComputeBuffer.ComputeBuffer);
            }

            float minDistance = 0;

            if(countDispatch > 0)
            {
                if(lodGroup.EnabledLODFade)
                {
                    minDistance = renderModel.LODs[4].Distance * lodBias - lodGroup.LodFadeTransitionDistance;
                }
                else
                {
                    minDistance = renderModel.LODs[4].Distance * lodBias;
                }
            }

            _frustumCulling.SetInt(_startLOD, countDispatch == 0 ? 0 : 4);
            _frustumCulling.SetFloat(_minDistanceID, minDistance);

            _frustumCulling.Dispatch(_frustumKernelHandle, threadGroups, 1, 1);
        }
        
        public static void Dispose()
        {
            _dummyComputeBuffer.DisposeUnmanagedMemory();
        }
    }
}