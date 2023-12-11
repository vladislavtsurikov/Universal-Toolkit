using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime.UnmanagedProperties;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    public class ComputeBufferRenderData 
    {   
        public ComputeBufferProperty ComputeBufferProperty = new ComputeBufferProperty();

        public void ConvertPersistentDataToRenderData(NativeArray<InstanceShaderData> instanceShaderDataArray)
        {
            if(instanceShaderDataArray.Length != 0)
            {
                int length = instanceShaderDataArray.Length;
                
                ComputeBufferProperty.ChangeComputeBuffer(new ComputeBuffer(length, Marshal.SizeOf(typeof(InstanceShaderData))));
                ComputeBufferProperty.ComputeBuffer.SetData(instanceShaderDataArray);
            }
        }
    }
}