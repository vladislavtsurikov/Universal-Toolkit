using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;
using VladislavTsurikov.SceneUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    [Serializable]
    public sealed class Sector
    {
        [NonSerialized] private ObjectBoundsBVHTree _objectBoundsBvhTree = new ObjectBoundsBVHTree();
        
        //References to GameObject in another SceneReference.Scene
        private List<GameObject> _references = new List<GameObject>();

        public Bounds Bounds;
        [OdinSerialize]
        public SceneReference SceneReference = new SceneReference();
        
        public bool CachedScene => SceneReference.CachedScene;

        public SceneDataManager SceneDataManager
        {
            get
            {
                SceneDataManager sceneDataManager = null;

                _references.RemoveAll(go => go == null);
                
                foreach (var go in _references)
                {
                    sceneDataManager = (SceneDataManager)go.GetComponentInChildren(typeof(SceneDataManager));
                }
                
                if (sceneDataManager == null)
                {
                    sceneDataManager = (SceneDataManager)GameObjectUtility.FindObjectOfType(typeof(SceneDataManager), SceneReference.Scene);

                    if (sceneDataManager == null)
                    {
                        return null;
                    }
                    
                    _references.Add(sceneDataManager.gameObject); 
                }

                return sceneDataManager;
            }
        }

        public AABB AABB => new(Bounds.center, Bounds.size);

        public bool IsLoaded => SceneReference.IsLoaded;

        public Sector(Scene scene, Bounds bounds, ObjectBoundsBVHTree objectBoundsBvhTree)
        {
            SceneReference = new SceneReference(scene);
            Bounds = bounds;
            Setup(objectBoundsBvhTree);
        }
        
#if UNITY_EDITOR
        public Sector(SceneAsset sceneAsset, Bounds bounds, ObjectBoundsBVHTree objectBoundsBvhTree)
        {
            SceneReference = new SceneReference(sceneAsset);
            Bounds = bounds;
            Setup(objectBoundsBvhTree);
        }
#endif

        public void Setup(ObjectBoundsBVHTree objectBoundsBvhTree)
        {
            _objectBoundsBvhTree = objectBoundsBvhTree;
        }
        
        [OnDeserializing]
        private void OnDeserializing()
        {
            _references = new List<GameObject>();
        }

        public void LoadScene(float waitForSeconds = 0)
        {
            CoroutineRunner.StartCoroutine(SceneReference.LoadScene(waitForSeconds));
        }
        
        public void UnloadScene(float waitForSeconds = 0)
        {
            CoroutineRunner.StartCoroutine(SceneReference.UnloadScene(waitForSeconds));
        }

        public void CacheScene(float waitForSeconds = 0, float keepScene = 0)
        {
            SceneReference.CacheScene(waitForSeconds, keepScene);
        }

        public void AddReference(GameObject gameObject)
        {
            if (!_references.Contains(gameObject))
            {
                _references.Add(gameObject);
            }
        }
        
        public List<GameObject> GetReferences()
        {
            return new List<GameObject>(_references);
        }
        
        public GameObject GetReference(string gameObjectName)
        {
            List<GameObject> gameObjects = _references.FindAll(o => o.name == gameObjectName);

            if (gameObjects.Count == 0)
            {
                return null;
            }
            
            return gameObjects[0];
        }

        private void SetActiveGameObjects(bool setActive)
        {
            GameObject[] gameObjects = SceneReference.Scene.GetRootGameObjects();

            foreach (var go in gameObjects)
            {
                go.SetActive(setActive);
            }
        }

        public void ChangeObjectBoundsNodeSize()
        {
            _objectBoundsBvhTree.ChangeNodeSize(this, SceneObjectsBoundsUtility.GetSceneObjectsBounds(this));
            
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                SceneReference.MarkSceneDirty();
            }
#endif
        }


        public bool IsValid()
        {
#if UNITY_EDITOR
            return SceneReference.IsValid();
#else
            return true;
#endif
        }
    }
}