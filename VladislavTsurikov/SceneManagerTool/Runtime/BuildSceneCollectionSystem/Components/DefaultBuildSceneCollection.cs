using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem.Components
{
    [MenuItem("Build Scene Collection")]
    public class DefaultBuildSceneCollection : BuildSceneCollection
    {
        [OdinSerialize] 
        public SceneCollectionList SceneCollectionList = new SceneCollectionList();
        
        protected override void SetupElement(object[] args = null)
        {
            SceneCollectionList.Setup();
        }

        public override List<SceneReference> GetSceneReferences()
        {
            return SceneCollectionList.GetSceneReferences();
        }

        public override List<SceneCollection> GetStartupSceneCollections()
        {
            return SceneCollectionList.FindAll(sceneCollection => sceneCollection.Startup);
        }

        public override List<SceneCollection> GetAllSceneCollections()
        {
            return new List<SceneCollection>(SceneCollectionList.ElementList);
        }
    }
}