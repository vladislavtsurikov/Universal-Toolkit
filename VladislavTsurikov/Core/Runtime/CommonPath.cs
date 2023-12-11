namespace VladislavTsurikov.Core.Runtime
{
    public static class CommonPath
    {
        private static readonly string Resources = CombinePath("Resources", Publisher);
        
        public const string Publisher = "VladislavTsurikov";
        
        public static readonly string PathToResources = CombinePath("Assets", Resources);
        public static readonly string PathToShaderVariantCollection = CombinePath(PathToResources, ShaderVariantCollectionName);
        
        public const string ShaderVariantCollectionName = "Shader Variant Collection";
        
        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}