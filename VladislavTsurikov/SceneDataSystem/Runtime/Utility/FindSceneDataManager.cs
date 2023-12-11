using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;

namespace VladislavTsurikov.SceneDataSystem.Runtime.Utility
{
    public static class FindSceneDataManager
    {
        public static List<SceneDataManager> OverlapSphere(Vector3 center, float radius, string sectorLayerTag = null)
        {
            if (SceneManager.sceneCount == 1)
            {
                return new List<SceneDataManager> { SectorLayerManager.ActiveSceneSector.SceneDataManager };
            }

            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();

            foreach (var sector in FindSector.OverlapSphere(center, radius, sectorLayerTag))
            {
                if (sector.IsLoaded)
                {
                    SceneDataManager sceneDataManager = sector.SceneDataManager;
                    
                    if(sceneDataManager != null)
                        sceneDataManagers.Add(sector.SceneDataManager);
                }
            }

            return sceneDataManagers;
        }

        public static List<SceneDataManager> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, string sectorLayerTag = null)
        {
            if (SceneManager.sceneCount == 1)
            {
                return new List<SceneDataManager> { SectorLayerManager.ActiveSceneSector.SceneDataManager };
            } 
            
            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();
            
            foreach (var sector in FindSector.OverlapBox(boxCenter, boxSize, boxRotation, sectorLayerTag))
            {
                if (sector.IsLoaded)
                {
                    SceneDataManager sceneDataManager = sector.SceneDataManager;
                    
                    if(sceneDataManager != null)
                        sceneDataManagers.Add(sector.SceneDataManager);
                }
            }

            return sceneDataManagers;
        }
        
        public static List<SceneDataManager> OverlapPosition(Vector3 position, string sectorLayerTag, bool objectBounds)
        {
            if (SceneManager.sceneCount == 1)
            {
                return new List<SceneDataManager> { SectorLayerManager.ActiveSceneSector.SceneDataManager };
            } 
            
            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();
            
            foreach (var sector in FindSector.OverlapPosition(position, sectorLayerTag, objectBounds))
            {
                if (sector.IsLoaded)
                {
                    SceneDataManager sceneDataManager = sector.SceneDataManager;
                    
                    if(sceneDataManager != null)
                        sceneDataManagers.Add(sector.SceneDataManager);
                }
            }

            return sceneDataManagers;
        }

        public static List<SceneDataManager> RaycastAll(Ray ray, string sectorLayerTag = null)
        {
            if (SceneManager.sceneCount == 1)
            {
                return new List<SceneDataManager> { SectorLayerManager.ActiveSceneSector.SceneDataManager };
            } 
            
            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();
            
            List<SectorLayer> sectorLayers = SectorLayer.GetCurrentSectorLayers(sectorLayerTag);
            
            if (sectorLayers == null)
                return sceneDataManagers;
            
            foreach (var sectorLayer in sectorLayers)
            {
                foreach (var sector in sectorLayer.ObjectBoundsBvhTree.RaycastAll(ray))
                {
                    if (sector.IsLoaded)
                    {
                        SceneDataManager sceneDataManager = sector.SceneDataManager;
                    
                        if(sceneDataManager != null)
                            sceneDataManagers.Add(sector.SceneDataManager);
                    }
                }
            }

            return sceneDataManagers;
        }
    }
}