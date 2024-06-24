using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using LOD = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.LOD;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender
{
    public class LODRenderData : IDisposable
    {
        public ComputeBufferProperty PositionBuffer;
        public ComputeBufferProperty PositionShadowBuffer;

        public List<ComputeBufferProperty> ArgsBufferMergedLODList = new List<ComputeBufferProperty>();
        public List<ComputeBufferProperty> ShadowArgsBufferMergedLODList = new List<ComputeBufferProperty>();

        public LODRenderData(LOD lod)
        {
            PositionBuffer = new ComputeBufferProperty(new ComputeBuffer(5000, 16 * 4 * 2 + 16, ComputeBufferType.Append)); 
            PositionBuffer.ComputeBuffer.SetCounterValue(0);

            PositionShadowBuffer = new ComputeBufferProperty(new ComputeBuffer(5000, 16 * 4 * 2 + 16, ComputeBufferType.Append));
            PositionShadowBuffer.ComputeBuffer.SetCounterValue(0);

            uint[] args = { 0, 0, 0, 0, 0 };

            Mesh mesh = lod.Mesh;

            for (int subMeshIndex = 0; subMeshIndex <= mesh.subMeshCount - 1; subMeshIndex++)
            {
                args[0] = mesh.GetIndexCount(subMeshIndex);
                args[2] = mesh.GetIndexStart(subMeshIndex);

                ComputeBufferProperty argsBufferMergedLOD = new ComputeBufferProperty(new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments));                             
                argsBufferMergedLOD.ComputeBuffer.SetData(args);
                ArgsBufferMergedLODList.Add(argsBufferMergedLOD);

                ComputeBufferProperty shadowArgsBufferMergedLOD = new ComputeBufferProperty(new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments));                             
                shadowArgsBufferMergedLOD.ComputeBuffer.SetData(args);
                ShadowArgsBufferMergedLODList.Add(shadowArgsBufferMergedLOD);
            }
        }

        public void Dispose()
        {
            PositionBuffer.DisposeUnmanagedMemory();
            PositionShadowBuffer.DisposeUnmanagedMemory();

            foreach (var item in ArgsBufferMergedLODList)
            {
                item.DisposeUnmanagedMemory();
            }

            foreach (var item in ShadowArgsBufferMergedLODList)
            {
                item.DisposeUnmanagedMemory();
            }
        }
    }
}