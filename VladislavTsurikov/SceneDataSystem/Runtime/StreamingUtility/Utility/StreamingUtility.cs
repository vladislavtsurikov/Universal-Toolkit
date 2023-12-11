using System.Collections;
using System.Collections.Generic;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility
{
    public static class StreamingUtility
    {
        public static void LoadScenes(List<Sector> sectors)
        {
            foreach (var sector in sectors)
            {
                sector.LoadScene();
            }
        }

        public static void CacheScenes(List<Sector> sectors, float waitForSeconds = 0F, float keepScene = 0F)
        {
            foreach (var sector in sectors)
            {
                sector.CacheScene(waitForSeconds, keepScene);
            }
        }

        public static void UnloadScenes(List<Sector> sectors, float waitForSeconds = 0F)
        {
            foreach (var sector in sectors)
            {
                sector.UnloadScene(waitForSeconds);
            }
        }

        public static IEnumerator UnloadAllScenes(string tag)
        {
            foreach (var sceneDataManager in SectorLayerManager.Instance.SectorLayerList)
            {
                if (sceneDataManager.Tag == tag)
                {
                    foreach (var sector in sceneDataManager.SectorList)
                    {
                        yield return sector.SceneReference.UnloadScene();
                    }
                }
            }
        }

        public static IEnumerator LoadAllScenes(string tag)
        {
            foreach (var sceneDataManager in SectorLayerManager.Instance.SectorLayerList)
            {
                if (sceneDataManager.Tag == tag)
                {
                    foreach (var sector in sceneDataManager.SectorList)
                    {
                        yield return sector.SceneReference.LoadScene();
                    }
                }
            }
        }

        public static List<Sector> GetAllScenes(string tag)
        {
            SectorLayer sectorLayer = SectorLayerManager.Instance.GetSectorLayer(tag);

            if (sectorLayer == null)
            {
                return new List<Sector>();
            }

            return sectorLayer.SectorList;
        }
    }
}