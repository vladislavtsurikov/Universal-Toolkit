using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect.GPUISupport;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.Attributes
{
    public class GenerateGPUInstancedIndirectShadersAttribute : ChangeShaderCodeAttribute
    {
        public override void ChangeShaderCode(Prototype proto)
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            GPUInstancedIndirectShaderStack.Instance.ClearEmptyShaders();

#if UNITY_EDITOR
            GPUInstancedIndirectShaderUtility.GenerateInstancedShadersIfNecessary(proto);
#endif
        }
    }
}