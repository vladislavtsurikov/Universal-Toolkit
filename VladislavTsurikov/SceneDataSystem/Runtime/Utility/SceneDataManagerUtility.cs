using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.Utility
{
    public static class SceneDataManagerUtility
    {
        public static void InstanceSceneDataManagerForAllScenes()
        {
            foreach (Scene scene in SceneManagement.GetAllScenes()) 
            {
                if (!scene.isLoaded)
                {
                    continue;
                }
                
                InstanceSceneDataManager(scene);   
            }
        }

        public static SceneDataManager InstanceSceneDataManager(Scene scene)
        {
            SceneDataManager sceneDataManager = SceneDataManagerFinder.Find(scene, false);

            if(sceneDataManager == null)
            {
                GameObject go = new GameObject("Scene Data Manager")
                {
                    hideFlags = HideFlags.HideInHierarchy 
                };
                
                SceneManager.MoveGameObjectToScene(go, scene);

                sceneDataManager = go.AddComponent<SceneDataManager>();

                if (sceneDataManager == null)
                {
                    sceneDataManager = go.GetComponent<SceneDataManager>();
                }
            }

            return sceneDataManager;
        }

        public static List<SceneDataManager> GetAllSceneDataManager(bool getActive = true)
        {
            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                SceneDataManager sceneDataManager = SceneDataManagerFinder.Find(scene, getActive);

                if (sceneDataManager != null)
                {
                    sceneDataManagers.Add(sceneDataManager);
                }
            }

            return sceneDataManagers;
        }

        public static void SetupAllSceneDataManager(bool forceSetupSceneDatas = false)
        {
            List<SceneDataManager> sceneDataManagers = GetAllSceneDataManager();
            
            for (int i = 0; i < sceneDataManagers.Count; i++)
            {
                sceneDataManagers[i].Setup(forceSetupSceneDatas); 
            }
        }
    }
}