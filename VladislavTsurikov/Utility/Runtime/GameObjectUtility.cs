using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Utility.Runtime.Extensions;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class GameObjectUtility
    {
        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }
        
        public static GameObject GetPrefabRoot(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }
            
#if UNITY_EDITOR
            if(PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab)
            {
                return gameObject;
            }

            return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
            return gameObject;
#endif
        }

        public static void ParentGameObject(GameObject gameObject, GameObject parent)
        {
            if (gameObject != null && parent != null)
            {
                gameObject.transform.SetParent(parent.transform, true);
            }
        }
        
        public static GameObject FindParentGameObject(string gameObjectName, Scene scene)
        {
            GameObject container = null;
            
            GameObject[] sceneRoots = scene.GetRootGameObjects();
			foreach(GameObject root in sceneRoots)
			{
				if(root.name == gameObjectName) 
                {
					container = root.transform.gameObject;
                    break;
				}
			} 

            if (container == null)
            {
                GameObject childObject = new GameObject(gameObjectName);
                SceneManager.MoveGameObjectToScene(childObject, scene);
                container = childObject.transform.gameObject;
            }

            return container;
        }

        public static Object FindObjectOfType(Type type, Scene scene, bool getActiveGameObject = false)
        {
            List<Object> components = FindObjectsOfType(type, scene, getActiveGameObject);

            if(components.Count == 0)
                return null;

            return components[0];
        }

        public static List<Object> FindObjectsOfType(Type type, bool getActiveGameObject = false)
        {
            List<Object> objects = new List<Object>();
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                objects.AddRange(FindObjectsOfType(type, scene, getActiveGameObject));
            }

            return objects;
        }

        public static List<Object> FindObjectsOfType(Type type, Scene scene, bool getActiveGameObject = false)
        {
            if(!scene.isLoaded)
            {
                return new List<Object>();
            }
            
            List<Object> componentList = new List<Object>();

            foreach (var gameObject in GetSceneObjects(scene))
            {
                if (getActiveGameObject)
                {
                    if(!gameObject.activeInHierarchy)
                        continue;
                }

                componentList.AddRange(gameObject.GetComponents(type));
            }

            return componentList;
        }
        
        public static GameObject[] GetSceneObjects(Scene scene)
        {
            var roots = new List<GameObject>(Mathf.Max(1, scene.rootCount));
            scene.GetRootGameObjects(roots);
            List<GameObject> sceneObjects = new List<GameObject>(Mathf.Max(1, scene.rootCount * 5));

            foreach (var root in roots)
            {
                var allChildrenAndSelf = root.GetAllChildrenAndSelf();
                sceneObjects.AddRange(allChildrenAndSelf);
            }

            return sceneObjects.ToArray();
        }    
        
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
			
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst;
        }
        
        public static void Unspawn(List<GameObject> unspawnPrefabs)
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();

            for (int index = 0; index < allGameObjects.Length; index++)
            {
                foreach (var prefab in unspawnPrefabs)
                {
                    if (IsSameGameObject(prefab, allGameObjects[index]))
                    {
                        Object.DestroyImmediate(allGameObjects[index]);
                        break;
                    }
                }
            }
        }
    }
}