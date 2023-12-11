using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime.UnmanagedProperties;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem;
using LOD = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.LOD;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem.Components.ObjectCameraRender
{
    public class PrototypeRenderData : IDisposable
    {
        public readonly ComputeBufferProperty MergeBuffer;

        public readonly List<LODRenderData> LODRenderDataList = new List<LODRenderData>();
        
        public PrototypeRenderData(List<LOD> LODs)
        {
            MergeBuffer = new ComputeBufferProperty(new ComputeBuffer(5000, Marshal.SizeOf(typeof(InstanceShaderData)), ComputeBufferType.Append));
            MergeBuffer.ComputeBuffer.SetCounterValue(0);

            for (int i = 0; i < LODs.Count; i++)
            {
                LODRenderDataList.Add(new LODRenderData(LODs[i]));   
            }
        }

        public void UpdateComputeBufferSize(int newInstanceCount)
        {
            if(newInstanceCount == 0)
            {
                newInstanceCount = 1;
            }

            MergeBuffer.ChangeComputeBuffer(new ComputeBuffer(newInstanceCount, Marshal.SizeOf(typeof(InstanceShaderData)), ComputeBufferType.Append));
            MergeBuffer.ComputeBuffer.SetCounterValue(0);

            for (int i = 0; i < LODRenderDataList.Count; i++)
            {
                LODRenderDataList[i].PositionBuffer.ChangeComputeBuffer(new ComputeBuffer(newInstanceCount, Marshal.SizeOf(typeof(InstanceShaderData)), ComputeBufferType.Append));
                LODRenderDataList[i].PositionBuffer.ComputeBuffer.SetCounterValue(0);

                LODRenderDataList[i].PositionShadowBuffer.ChangeComputeBuffer(new ComputeBuffer(newInstanceCount, Marshal.SizeOf(typeof(InstanceShaderData)), ComputeBufferType.Append));
                LODRenderDataList[i].PositionShadowBuffer.ComputeBuffer.SetCounterValue(0);
            }
        }
        
        public void ClearRenderData()
        {
            for (int lodIndex = 0; lodIndex < LODRenderDataList.Count; lodIndex++)
            {
                LODRenderDataList[lodIndex].PositionBuffer.ComputeBuffer.SetCounterValue(0); 
                LODRenderDataList[lodIndex].PositionShadowBuffer.ComputeBuffer.SetCounterValue(0); 

                for (int n = 0; n < LODRenderDataList[lodIndex].ArgsBufferMergedLODList.Count; n++)
                {
                    ComputeBuffer.CopyCount(LODRenderDataList[lodIndex].PositionBuffer.ComputeBuffer, LODRenderDataList[lodIndex].ArgsBufferMergedLODList[n].ComputeBuffer, sizeof(uint) * 1);
                    ComputeBuffer.CopyCount(LODRenderDataList[lodIndex].PositionShadowBuffer.ComputeBuffer, LODRenderDataList[lodIndex].ShadowArgsBufferMergedLODList[n].ComputeBuffer, sizeof(uint) * 1);
                }
            }
        }

        public void Dispose()
        {
            MergeBuffer.DisposeUnmanagedMemory();
            foreach (var lodRenderData in LODRenderDataList)
            {
                lodRenderData.Dispose();
            }
        }
    }
}