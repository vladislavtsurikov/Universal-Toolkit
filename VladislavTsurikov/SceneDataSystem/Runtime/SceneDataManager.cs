using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [ExecuteInEditMode]
    public class SceneDataManager : SerializedMonoBehaviour
    {
        [NonSerialized]
        private Sector _sector;
        [OdinSerialize]
        private SceneType _sceneType;

        public SceneType SceneType
        {
            get
            {
                return _sceneType;
            }
            set
            {
                if (_sceneType != value)
                {
                    _sceneType = value;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(Scene);
                    }
#endif
                }
            }
        }
            
        public SceneDataStack SceneDataStack = new SceneDataStack();
        public Scene Scene => gameObject.scene;
        
        public bool IsSetup { get; private set; }

        public Sector Sector
        {
            get
            {
                if (SceneManager.sceneCount == 1)
                {
                    return null;
                }

                if (_sector == null)
                {
                    SectorLayerManager sectorLayerManager = SectorLayerManager.Instance;

                    if (sectorLayerManager == null)
                    {
                        return null;
                    }
                    
                    sectorLayerManager.Setup();

                    foreach (var sectorLayer in sectorLayerManager.SectorLayerList)
                    {
                        foreach (var sector in sectorLayer.SectorList)
                        {
                            if (sector.SceneReference.SceneName == Scene.name)
                            {
                                _sector = sector;
                                break;
                            }
                        }
                    }
                }
                
                return _sector;
            }
        }

        protected override void OnAfterDeserialize()
        {
            if (SceneDataStack == null)
            {
                SceneDataStack = new SceneDataStack();
            }
        }
        
        private void Awake()
        {
            transform.SetSiblingIndex(0);
        }

        private void OnEnable()
        {
            Setup(true); 
        }

        private void OnDisable() 
        {
            IsSetup = false;
            
            SceneDataStack.OnDisable();
        }
        
        private void LateUpdate()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            if (!IsSetup)
            {
                Setup();
            }

            for (int i = 0; i < SceneDataStack.ElementList.Count; i++)
            {
                SceneDataStack.ElementList[i]?.LateUpdate();
            }
        }
        
        public async UniTask Setup(bool forceSetup = false, CancellationToken cancellationToken = default)
        {
            if (!gameObject.scene.isLoaded || !gameObject.activeInHierarchy)
            {
                return;
            }

#if UNITY_EDITOR
            if (RemoveMultipleSceneDataManagers())
            {
                return;
            }
#endif

            cancellationToken.ThrowIfCancellationRequested();

            IsSetup = true;

            await SceneDataStack.Setup(forceSetup,new object[]{this}, cancellationToken);
        }

        public bool RemoveMultipleSceneDataManagers()
        {
            GameObject[] sceneRoots = Scene.GetRootGameObjects();
            List<GameObject> sceneDataManagers = new List<GameObject>();
            foreach(GameObject root in sceneRoots)
            {
                if(root.name == "Scene Data Manager") 
                {
                    sceneDataManagers.Add(root);
                }
            }

            bool happenedDestroy = false;

            if (sceneDataManagers.Count > 1)
            {
                for (int i = sceneDataManagers.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        break;
                    }
                    
                    DestroyImmediate(sceneDataManagers[i].gameObject);
                    happenedDestroy = true;
                }
            }

            return happenedDestroy;
        }
    }
}