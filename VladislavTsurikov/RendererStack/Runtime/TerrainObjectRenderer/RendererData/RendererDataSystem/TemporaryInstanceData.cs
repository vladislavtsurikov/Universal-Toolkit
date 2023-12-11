using System.Collections.Generic;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    public class TemporaryInstanceData
    {
        public NativeRenderData NativeRenderData = new NativeRenderData();
        public ComputeBufferRenderData ComputeBufferRenderData = new ComputeBufferRenderData();
        
        public void ConvertPersistentDataToTemporaryData(List<RendererInstance> instanceList)
        {
            NativeRenderData.ConvertPersistentDataToRenderData(instanceList);
            ComputeBufferRenderData.ConvertPersistentDataToRenderData(NativeRenderData.InstanceShaderDataList.NativeArray);
        }

        public void DisposeUnmanagedMemory()
        {
            NativeRenderData.InstanceShaderDataList.DisposeUnmanagedMemory();
            ComputeBufferRenderData.ComputeBufferProperty.DisposeUnmanagedMemory();
        }
    }
}