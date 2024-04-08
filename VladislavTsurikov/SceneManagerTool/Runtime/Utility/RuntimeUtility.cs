using System.Collections;
using UnityEngine;
using VladislavTsurikov.Coroutines.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    internal static class RuntimeUtility
    {
        internal static void Start()
        {
            CoroutineRunner.StartCoroutine(Load(), SceneManagerData.Instance);
            
            static IEnumerator Load()
            {
                foreach (var sceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ActiveBuildSceneCollection.GetStartupSceneCollections())
                {
                    yield return sceneCollection.Load();
                }
            }
        }
    }
}