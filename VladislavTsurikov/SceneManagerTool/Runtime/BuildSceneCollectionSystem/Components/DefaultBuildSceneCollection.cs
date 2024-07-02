using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem
{
    [Name("Build Scene Collection")]
    public class DefaultBuildSceneCollection : BuildSceneCollection
    {
        [OdinSerialize] 
        public SceneCollectionStack SceneCollectionStack = new SceneCollectionStack();
        
        protected override void SetupComponent(object[] setupData = null)
        {
            SceneCollectionStack.Setup();
        }

        public override List<SceneReference> GetSceneReferences()
        {
            return SceneCollectionStack.GetSceneReferences();
        }

        public override List<SceneCollection> GetStartupSceneCollections()
        {
            return SceneCollectionStack.FindAll(sceneCollection => sceneCollection.Startup);
        }

        public override List<SceneCollection> GetAllSceneCollections()
        {
            return new List<SceneCollection>(SceneCollectionStack.ElementList);
        }
    }
}