using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    internal static class RuntimeUtility
    {
        internal static void Start()
        {
            Load().Forget();
            
            static async UniTask Load()
            {
                foreach (var sceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ActiveBuildSceneCollection.GetStartupSceneCollections())
                {
                    await sceneCollection.Load();
                }
            }
        }
    }
}