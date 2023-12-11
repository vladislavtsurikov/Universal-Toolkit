using System.Collections.Generic;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    public static class SceneCollectionUtility
    {
        public static SceneCollection GetSceneCollection(int id) 
        {
            foreach (var buildSceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionList.ElementList)
            {
                foreach (var sceneCollection in buildSceneCollection.GetAllSceneCollections())
                {
                    if (sceneCollection.ID == id)
                        return sceneCollection;
                }
            }

            return null;
        }

        public static List<SceneCollection> GetAllCurrentSceneCollection()
        {
            List<SceneCollection> sceneCollections = new List<SceneCollection>();
            foreach (var buildSceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionList.ElementList)
            {
                foreach (var sceneCollection in buildSceneCollection.GetAllSceneCollections())
                {
                    sceneCollections.Add(sceneCollection);
                }
            }

            return sceneCollections;
        }
    }
}