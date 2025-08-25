using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem
{
    [Name("Build Scene Collection")]
    public class DefaultBuildSceneCollection : BuildSceneCollection
    {
        [OdinSerialize]
        public SceneCollectionStack SceneCollectionStack = new();

        protected override UniTask SetupComponent(object[] setupData = null) => SceneCollectionStack.Setup();

        public override List<SceneReference> GetSceneReferences() => SceneCollectionStack.GetSceneReferences();

        public override List<SceneCollection> GetStartupSceneCollections() =>
            SceneCollectionStack.FindAll(sceneCollection => sceneCollection.Startup);

        public override List<SceneCollection> GetAllSceneCollections() => new(SceneCollectionStack.ElementList);
    }
}
