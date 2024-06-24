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
            SceneDataManager sceneDataManager = FindSceneDataManager(scene, false);

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
                SceneDataManager sceneDataManager = FindSceneDataManager(scene, getActive);

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

        //The Object.FindObjectOfType method does not allow me to find a component with a hidden GameObject, but this method allows me to
        public static SceneDataManager FindSceneDataManager(Scene scene, bool getActive = true)
        {
            Profiler.BeginSample("FindSceneDataManager");
            
            if (!scene.isLoaded)
            {
                Profiler.EndSample();
                return null;
            }
            
            GameObject[] gameObjects = scene.GetRootGameObjects();

            foreach (GameObject go in gameObjects)
            {
                if (getActive)
                {
                    if (!go.activeInHierarchy)
                    {
                        continue;
                    }
                }
                
                Object obj = go.GetComponentInChildren(typeof(SceneDataManager));
                if(obj != null)
                {
                    SceneDataManager sceneDataManager = (SceneDataManager)obj;
                    if (!sceneDataManager.IsSetup)
                    {
                        sceneDataManager.Setup(); 
                    }
                    
                    Profiler.EndSample();

                    return sceneDataManager;
                }
            }
            
            Profiler.EndSample();

            return null;
        }
    }
}