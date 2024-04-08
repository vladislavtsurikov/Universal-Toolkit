using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem.Jobs
{
    [BurstCompile(CompileSynchronously = true)]
    public struct ConvertToMatrixJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Instance> InstanceList;
        public NativeArray<Matrix4x4> MatrixList;

        public void Execute(int index)
        {
            MatrixList[index] = Matrix4x4.TRS(InstanceList[index].Position, InstanceList[index].Rotation, InstanceList[index].Scale);
        }
    }
}              