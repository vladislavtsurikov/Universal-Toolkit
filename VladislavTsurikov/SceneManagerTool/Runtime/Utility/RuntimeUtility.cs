using System.Collections;
using VladislavTsurikov.Coroutines.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    internal static class RuntimeUtility
    {
        internal static void Start()
        {
            CoroutineRunner.StartCoroutine(Run(), SceneManagerData.Instance);
        }

        private static IEnumerator Run()
        {
            foreach (var sceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionList.ActiveBuildSceneCollection.GetStartupSceneCollections())
            {
                yield return sceneCollection.Load();
            }
        }
    }
}