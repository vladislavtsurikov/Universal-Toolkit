using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core
{
    public static class RendererStackConstants 
    {
        public static readonly float Version = 1.05f; 

        private const string RendererStack = "RendererStack";
        private const string ShadersName = "Shaders";

        private static readonly string _pathToResourcesRendererStack = CommonPath.CombinePath(CommonPath.PathToResources, RendererStack);
        public static readonly string PathToShaders = CommonPath.CombinePath(_pathToResourcesRendererStack, ShadersName);
        
        #region Shaders
        public static readonly string ShaderUnityInternalError = "Hidden/InternalErrorShader";
        #endregion Shaders
    }
}