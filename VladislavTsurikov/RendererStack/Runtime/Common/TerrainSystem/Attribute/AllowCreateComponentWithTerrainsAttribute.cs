using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Attributes;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem.Attribute
{
    internal class AllowCreateComponentWithTerrainsAttribute : AllowCreateComponentAttribute
    {
        public override bool Allow(SceneDataManager sceneDataManager)
        {
            TerrainManager terrainManager = SceneDataStackUtility.InstanceSceneData<TerrainManager>(sceneDataManager.Scene);

            if(terrainManager != null)
            {
                if (!terrainManager.IsSetup)
                {
                    sceneDataManager.SceneDataStack.SetupElement<TerrainManager>();
                }
                
                if(terrainManager.TerrainHelperList.Count == 0) 
                {
                    return false;
                }
            }

            return true; 
        }
    }
}
