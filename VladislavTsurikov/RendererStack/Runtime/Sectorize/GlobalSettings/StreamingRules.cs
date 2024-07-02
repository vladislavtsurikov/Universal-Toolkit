using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings
{
    [Name("Streaming Rules")]
    public class StreamingRules : GlobalComponent
    {
        public float MaxImmediatelyLoadingDistance = 2000;
        public float OffsetMaxLoadingDistanceWithPause = 2000;
        public float OffsetMaxDistancePreventingUnloadScene = 400;
        
        public float MaxPauseBeforeLoadingScene = 1;

        public bool UseCaching = true;
        public float MaxPauseBeforeLoadingCachedScene = 0.2f;
        public float KeepScenes = 300;

        public float GetMaxPauseBeforeLoadingScene(Sector sector)
        {
            if (sector.CachedScene)
            {
                return MaxPauseBeforeLoadingCachedScene;
            }

            return MaxPauseBeforeLoadingScene;
        }

        public float GetMaxLoadingDistance()
        {
            return MaxImmediatelyLoadingDistance + OffsetMaxLoadingDistanceWithPause;
        }
    }
}