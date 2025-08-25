using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    [RequiredSceneType(SceneType.ParentScene)]
    public class SectorLayerManager : SingletonSceneData<SectorLayerManager>
    {
        private static Sector _activeSceneSector;

        [OdinSerialize] 
        public List<SectorLayer> SectorLayerList = new List<SectorLayer>();

        public static Sector ActiveSceneSector
        {
            get
            {
                if (_activeSceneSector == null || !_activeSceneSector.SceneReference.Scene.isLoaded) 
                {
                    _activeSceneSector = new Sector(SceneManager.GetActiveScene(), new Bounds(), null);
                }

                return _activeSceneSector;
            }
        }

        protected override void SetupSceneData()
        {
            foreach (var sectorLayer in SectorLayerList)
            {
                sectorLayer.Setup();
            }

            SectorLayerList.RemoveAll(layer => layer.SectorList.Count == 0);
        }

        public SectorLayer GetSectorLayer(string tag)
        {
            foreach (var sectorLayer in SectorLayerList)
            {
                if (sectorLayer.Tag == tag)
                {
                    return sectorLayer;
                }
            }

            return null;
        }

        public Sector GetSector(Scene scene)
        {
            foreach (var sectorLayer in SectorLayerList)
            {
                foreach (var sector in sectorLayer.SectorList)
                {
                    if (scene.name == sector.SceneReference.Scene.name)
                    {
                        return sector;
                    }
                }
            }

            return null;
        }

        public List<Sector> GetLoadedScenes(string tag)
        {
            foreach (var sectorLayer in SectorLayerList)
            {
                if (sectorLayer.Tag == tag)
                {
                    return sectorLayer.GetLoadedScenes();
                }
            }

            return null;
        }
        
#if UNITY_EDITOR
        public void RemoveSector(Sector sector)
        {
            foreach (var sectorLayer in SectorLayerList)
            {
                if (sectorLayer.SectorList.Remove(sector))
                {
                    return;
                }
            }
        }
        
        public void AddSector(string tag, SceneAsset sceneAsset, Bounds bounds)
        {
            SectorLayer sectorLayer = GetSectorLayer(tag);
            if (sectorLayer == null)
            {
                sectorLayer = new SectorLayer(tag);
                SectorLayerList.Add(sectorLayer);
            }

            sectorLayer.AddSector(sceneAsset, bounds);
        }
        
        public SceneReference CreateScene(string tag, string sceneName, Bounds bounds)
        {
            SectorLayer sectorLayer = GetSectorLayer(tag);
            if (sectorLayer == null)
            {
                sectorLayer = new SectorLayer(tag);
                SectorLayerList.Add(sectorLayer);
            }

            return sectorLayer.CreateScene(sceneName, bounds);
        }
#endif
    }
}