#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Editor.StreamingUtility
{
    public static class StreamingUtilityEditor
    {
        public static IEnumerator CreateScene(string tag, string sceneName, Bounds bounds, List<GameObject> moveGameObjectToScene)
        {
            SceneReference sceneReference = SectorLayerManager.Instance.CreateScene(tag, sceneName, bounds);

            yield return sceneReference.LoadScene();

            foreach (var gameObject in moveGameObjectToScene)
            {
                gameObject.transform.parent = null;
                
                SceneManager.MoveGameObjectToScene(gameObject, sceneReference.Scene);
            }
            
            StreamingUtilityEvents.CreateSceneAfterEvent?.Invoke();
            yield return null;
        }
        
        public static SceneReference CreateScene(string sceneName, string tag, Bounds bounds)
        {
            SceneReference sceneReference = SectorLayerManager.Instance.CreateScene(tag, sceneName, bounds);
            
            StreamingUtilityEvents.CreateSceneAfterEvent?.Invoke();
            
            return sceneReference;
        }

        public static void DeleteAllAdditiveScenes()
        {
            StreamingUtilityEvents.BeforeDeleteAllAdditiveScenesEvent.Invoke();
            
            if (SceneManager.sceneCount > 1)
            {
                foreach (SectorLayer sectorLayer in SectorLayerManager.Instance.SectorLayerList)
                {
                    for (int i = sectorLayer.SectorList.Count - 1; i >= 0; i--)
                    {
                        sectorLayer.DeleteScene(sectorLayer.SectorList[i]);
                    }
                }
            }
        }
    }
}
#endif