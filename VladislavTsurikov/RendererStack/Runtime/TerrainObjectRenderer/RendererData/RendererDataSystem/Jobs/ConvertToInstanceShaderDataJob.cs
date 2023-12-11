using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem.Jobs
{
#if RENDERER_STACK_BURST
    [BurstCompile(CompileSynchronously = true)]
#endif
    public struct ConvertToInstanceShaderDataJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<RendererInstance> InstanceArray;
        
        [NativeDisableUnsafePtrRestriction]
        public unsafe void* InstanceShaderDataPtr;

        public unsafe void Execute(int index)
        {
            ref InstanceShaderData instanceShaderData = ref UnsafeUtility.ArrayElementAsRef<InstanceShaderData>(InstanceShaderDataPtr, index);

            RendererInstance rendererInstance = InstanceArray[index];

            instanceShaderData.SetMatrix(float4x4.TRS(rendererInstance.Position, rendererInstance.Rotation, rendererInstance.Scale));
        }
    }
}