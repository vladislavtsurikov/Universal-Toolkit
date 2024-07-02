using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        private Scene _lastLoadedScene;

        public override void Render()
        {
            if (Application.isPlaying)
            {
                if (!SceneManagerIntegration.SectorizeStreamingScenes.StartupLoadComplete)
                {
                    return;
                }
            }
            else
            {
                if (!RendererStackManager.Instance.EditorPlayModeSimulation)
                {
                    return;
                }
            }
            
            CameraManager cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));
            StreamingRules streamingRules = (StreamingRules)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(StreamingRules), GetType());

            foreach (var cam in cameraManager.VirtualCameraList)
            {
                if (cam.Ignored) 
                {
                    continue;
                }

                List<Sector> loadSectors = FindSector.OverlapSphere(cam.Camera.transform.position, streamingRules.GetMaxLoadingDistance(), GetSectorLayerTag(), false);

                if (loadSectors == null)
                {
                    continue;
                }
                    
                foreach (var sector in loadSectors)
                {
                    sector.SceneReference.UnloadSceneCancellationTokenSource?.Cancel();

                    if (sector.IsLoaded)
                    {
                        continue;
                    }
                    
                    Sphere loadImmediatelySphere = new Sphere(cam.Camera.transform.position, streamingRules.MaxImmediatelyLoadingDistance);
                                    
                    if (loadImmediatelySphere.Intersects(new AABB(sector.Bounds.center, sector.Bounds.size)))
                    {
                        sector.LoadScene();
                        continue;
                    }
                    
                    if (!_lastLoadedScene.IsValid() || _lastLoadedScene.isLoaded)
                    {
                        sector.LoadScene(CalculateMaximumPauseBeforeLoadingScene(sector, cam.Camera.transform.position, streamingRules));
                        _lastLoadedScene = sector.SceneReference.Scene;
                    }
                }
                        
                Sphere preventingUnloadSceneSphere = new Sphere(cam.Camera.transform.position, streamingRules.GetMaxLoadingDistance() + streamingRules.OffsetMaxDistancePreventingUnloadScene);
                        
                UnloadUnnecessaryScenes(loadSectors, preventingUnloadSceneSphere, streamingRules);
            }
        }

        private void UnloadUnnecessaryScenes(List<Sector> loadSectors, Sphere preventingUnloadSceneSphere, StreamingRules streamingRules)
        {
            List<Sector> allLoadedScenes = SectorLayerManager.Instance.GetLoadedScenes(GetSectorLayerTag());

            if (allLoadedScenes == null)
            {
                return;
            }
            
            List<Sector> unloadSceneDatas = new List<Sector>(allLoadedScenes);
            unloadSceneDatas.RemoveAll(loadSectors.Contains);

            foreach (var sector in unloadSceneDatas)
            {
                if (!preventingUnloadSceneSphere.Intersects(new AABB(sector.Bounds.center, sector.Bounds.size)))
                {
                    if (streamingRules.UseCaching)
                    {
                        sector.CacheScene(0, streamingRules.KeepScenes);
                    }
                    else
                    {
                        sector.UnloadScene();
                    }
                }
            }
        }

        public List<Sector> GetLoadSectorsForSceneManager()
        {
            List<Sector> startupSectors = new List<Sector>();
            
            CameraManager cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));
            StreamingRules streamingRules = (StreamingRules)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(StreamingRules), GetType());
            
            foreach (var cam in cameraManager.VirtualCameraList)
            {
                if (cam.Ignored) 
                {
                    continue;
                }

                startupSectors.AddRange(FindSector.OverlapSphere(cam.Camera.transform.position, streamingRules.GetMaxLoadingDistance(), GetSectorLayerTag(), false));
            }

            return startupSectors;
        }

        private float CalculateMaximumPauseBeforeLoadingScene(Sector sector, Vector3 center, StreamingRules streamingRules)
        {
            float distance = Vector3.Distance(sector.Bounds.center, center);
            float t = Mathf.InverseLerp(streamingRules.GetMaxLoadingDistance(), streamingRules.MaxImmediatelyLoadingDistance, distance); 
                                        
            return Mathf.Lerp(streamingRules.GetMaxPauseBeforeLoadingScene(sector), 0, t);
        }
    }
}