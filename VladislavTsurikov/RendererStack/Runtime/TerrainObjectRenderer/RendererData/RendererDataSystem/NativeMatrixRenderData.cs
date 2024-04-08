using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime.UnmanagedProperties;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem.Jobs;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    public struct InstanceShaderData
    {
        public float4x4 Matrix;
        public float4x4 InverseMatrix;
        public float4 LODControl;

        public void SetMatrix(float4x4 matrix)
        {
            Matrix = matrix;
            InverseMatrix = math.inverse(Matrix);
            LODControl = new Vector4();
        }
    };
    
    public class NativeRenderData 
    {
        public NativeArrayProperty<InstanceShaderData> InstanceShaderDataList = new NativeArrayProperty<InstanceShaderData>(); 

        public unsafe void ConvertPersistentDataToRenderData(List<Instance> instances)
        {
            InstanceShaderDataList.ChangeNativeArray(new NativeArray<InstanceShaderData>());
            
            if(instances.Count != 0)
            {
                NativeArray<Instance> nativeItemArray = new NativeArray<Instance>(instances.Count, Allocator.Persistent);
                nativeItemArray.CopyFromFast(instances);
                
                InstanceShaderDataList.ChangeNativeArray(new NativeArray<InstanceShaderData>(instances.Count, Allocator.Persistent));
                
                ConvertToInstanceShaderDataJob loadPersistentStorageToMatrixJob =
                    new ConvertToInstanceShaderDataJob
                    {
                        InstanceArray = nativeItemArray,
                        InstanceShaderDataPtr = InstanceShaderDataList.NativeArray.GetUnsafePtr(),
                    };
                
                loadPersistentStorageToMatrixJob.Schedule(instances.Count, 64).Complete();

                nativeItemArray.Dispose();
            }
        }
    }
    
    public class NativeMatrixRenderData 
    {
        public NativeArrayProperty<Matrix4x4> MatrixList = new NativeArrayProperty<Matrix4x4>(); 

        public void ConvertPersistentDataToRenderData(List<Instance> instances)
        {
            MatrixList.ChangeNativeArray(new NativeArray<Matrix4x4>());
            
            if(instances.Count != 0)
            {
                NativeArray<Instance> nativeItemArray = new NativeArray<Instance>(instances.Count, Allocator.Persistent);
                nativeItemArray.CopyFromFast(instances);
                
                MatrixList.ChangeNativeArray(new NativeArray<Matrix4x4>(instances.Count, Allocator.Persistent));
                
                ConvertToMatrixJob loadPersistentStorageToMatrixJob =
                    new ConvertToMatrixJob
                    {
                        InstanceList = nativeItemArray,
                        MatrixList = MatrixList.NativeArray,
                    };

                loadPersistentStorageToMatrixJob.Schedule(instances.Count, 64).Complete();

                nativeItemArray.Dispose();
            }            
        }
    }
}