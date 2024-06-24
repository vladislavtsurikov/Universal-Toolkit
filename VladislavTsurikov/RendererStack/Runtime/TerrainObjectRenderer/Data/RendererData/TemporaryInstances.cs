using System.Collections.Generic;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    public class TemporaryInstances
    {
        public NativeRenderData NativeRenderData = new NativeRenderData();
        public ComputeBufferRenderData ComputeBufferRenderData = new ComputeBufferRenderData();
        
        public void ConvertPersistentDataToTemporaryData(List<Instance> instanceList)
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