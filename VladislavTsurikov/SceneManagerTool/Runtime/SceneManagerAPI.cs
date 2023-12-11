using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime
{
    public static class SceneManagerAPI
    {
        public static void UnloadSceneCollection(SceneCollection sceneCollection)
        {
            CoroutineRunner.StartCoroutine(sceneCollection.Unload(null));
        }

        public static void LoadSceneCollection(SceneCollection sceneCollection)
        {
            CoroutineRunner.StartCoroutine(sceneCollection.Load());
        }
        
        public static void LoadSceneType(SceneType sceneType)
        {
            CoroutineRunner.StartCoroutine(sceneType.LoadInternal(true));
        }

        public static void UnloadSceneComponent(SceneType sceneType)
        {
            CoroutineRunner.StartCoroutine(sceneType.UnloadInternal(null, true));
        }
    }
}