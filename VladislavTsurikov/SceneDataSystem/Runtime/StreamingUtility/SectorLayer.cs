using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.OdinSerializer.Core.Misc;

#if UNITY_EDITOR
using VladislavTsurikov.SceneUtility.Editor;
using UnityEditor;
#endif

using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    [Serializable]
    public class SectorLayer 
    {
        public string Tag;
        
        [OdinSerialize] 
        public List<Sector> SectorList = new List<Sector>();
        public SectorBVHTree SectorBvhTree { get; private set; } = new SectorBVHTree();

        public ObjectBoundsBVHTree ObjectBoundsBvhTree { get; private set; } = new ObjectBoundsBVHTree();

        public SectorLayer(string tag)
        {
            Tag = tag;
        }
        
        [OnDeserializing]
        private void Initialize()
        {
            SectorBvhTree = new SectorBVHTree();
            ObjectBoundsBvhTree = new ObjectBoundsBVHTree();
        }
        
        public void Setup()
        {
#if UNITY_EDITOR
            SectorList.RemoveAll(sector => sector == null || !sector.IsValid());
#endif
            
            SectorBvhTree.Clear();
            ObjectBoundsBvhTree.Clear();

            foreach (var sector in SectorList)
            {
                sector.Setup(ObjectBoundsBvhTree);
                
                SectorBvhTree.RegisterSector(sector); 
                ObjectBoundsBvhTree.RegisterSector(sector, SceneObjectsBoundsUtility.GetSceneObjectsBounds(sector));
            }
        }

        public void RemoveNullScene()
        {
            for (int i = SectorList.Count - 1; i >= 0; i--)
            {
                if (SectorList[i] == null || !SectorList[i].IsValid())
                {
                    SectorList.RemoveAt(i);
                }
            }
        }
        
        public static void ChangeObjectBoundsNodeSize(Scene scene)
        {
            foreach (var sectorLayer in SectorLayerManager.Instance.SectorLayerList)
            {
                foreach (var sector in sectorLayer.SectorList)
                {
                    if (sector.SceneReference.SceneName == scene.name)
                    {
                        sectorLayer.ObjectBoundsBvhTree.ChangeNodeSize(sector, SceneObjectsBoundsUtility.GetSceneObjectsBounds(sector));
                    }
                }
            }
        }

        public List<Sector> GetLoadedScenes()
        {
            List<Sector> loadedScene = new List<Sector>();

            foreach (var sector in SectorList)
            {
                if (sector.IsLoaded)
                {
                    loadedScene.Add(sector);
                }
            }

            return loadedScene;
        }
        
        public static List<SectorLayer> GetCurrentSectorLayers(string tag = null)
        {
            if (tag != null)
            {
                SectorLayer sectorLayer = SectorLayerManager.Instance.GetSectorLayer(tag);

                if (sectorLayer == null)
                {
                    return null;
                }

                return new List<SectorLayer> { sectorLayer };
            }
            else
            {
                return SectorLayerManager.Instance.SectorLayerList;
            }
        }

        private void RemoveSector(Sector sector)
        {
            if (SectorList.Remove(sector))
            {
                SectorBvhTree.RemoveNodes(sector);
            }
        }

#if UNITY_EDITOR
        private Sector AddSector(SceneAsset sceneAsset, Bounds bounds)
        {
            Sector sector = new Sector(sceneAsset, bounds, ObjectBoundsBvhTree); 
            SectorList.Add(sector);
            SectorBvhTree.RegisterSector(sector);
            
            return sector;
        }
        
        public SceneReference CreateScene(string sceneName, Bounds bounds)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            string pathToFolder = "Assets/Scenes/Streaming Utility/" + activeScene.name + "/" + Tag;
            
            SceneAsset sceneAsset = SceneCreationUtility.CreateScene(sceneName, pathToFolder, true);
            
            return AddSector(sceneAsset, bounds).SceneReference;
        }
        
        public void DeleteScene(Sector sector)
        {
            if (!SectorList.Contains(sector))
            {
                return;
            }
            
            RemoveSector(sector);
            
            Scene activeScene = SceneManager.GetActiveScene();
            
            var roots = new List<GameObject>(Mathf.Max(1, sector.SceneReference.Scene.rootCount));
            sector.SceneReference.Scene.GetRootGameObjects(roots);

            foreach (var gameObject in roots)
            {
                if (gameObject.GetComponent(typeof(SceneDataManager)) != null)
                {
                    continue;
                }
                        
                SceneManager.MoveGameObjectToScene(gameObject, activeScene);
            }
            
            CoroutineRunner.StartCoroutine(Coroutine());
            
            return;
            
            IEnumerator Coroutine()
            {
                yield return sector.SceneReference.UnloadScene();
                sector.SceneReference.DeleteAsset();
            }
        }
#endif
    }
}