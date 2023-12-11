using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility
{
    public static class SceneObjectsBoundsUtility
    {
        public static void ChangeSceneObjectsBounds(Sector sector)
        {
            if(sector == null)
                return;
            
            sector.ChangeObjectBoundsNodeSize();
        }

        public static AABB GetSceneObjectsBounds(Sector sector)
        {
            AABB currentAABB = sector.AABB;

            SceneDataManager sceneDataManager = sector.SceneDataManager;

            if (sceneDataManager == null || !sceneDataManager.IsSetup)
                return currentAABB;
            
            foreach (SceneData sceneData in sceneDataManager.SceneDataStack.ElementList)
            {
                if (sceneData is RendererSceneData rendererSceneData)
                {
                    currentAABB.Encapsulate(rendererSceneData.GetAABB()); 
                }
            }

            return currentAABB;
        }
    }
}