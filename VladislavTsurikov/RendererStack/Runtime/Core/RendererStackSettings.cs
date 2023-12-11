using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core
{
    [LocationAsset("RendererStack/RendererStackSettings")] 
    public class RendererStackSettings : SerializedScriptableObjectSingleton<RendererStackSettings>
    {
        public float Version;

        public bool AutoShaderConversion = true;
        public bool ShowRenderModelData;
        public bool RenderSceneCameraInPlayMode;
        public bool RenderDirectToCamera;
        
        public bool ShowColliders = true;
    }
}