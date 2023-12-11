#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect.GPUISupport;

namespace VladislavTsurikov.RendererStack.Editor.Core
{
    [InitializeOnLoad]
    public class VersionManager
    {
        static VersionManager()
        {
            if (RendererStackSettings.Instance.Version != RendererStackConstants.Version)
            {
                RendererStackSettings.Instance.Version = RendererStackConstants.Version;

                GPUInstancedIndirectShaderStack.Instance.RegenerateShaders();
                
                EditorUtility.SetDirty(RendererStackSettings.Instance);
            }
        }
    }
}
#endif