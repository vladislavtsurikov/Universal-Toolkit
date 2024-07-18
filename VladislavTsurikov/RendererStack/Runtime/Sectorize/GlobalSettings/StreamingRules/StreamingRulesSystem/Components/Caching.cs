using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Caching")]
    public class Caching : StreamingRule
    {
        public float MaxLoadingCachedScenePause = 0.2f;
        public float KeepScenes = 300;
    }
}