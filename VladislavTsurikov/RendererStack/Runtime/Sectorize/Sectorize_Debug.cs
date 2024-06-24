#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize 
    {
        public override void DrawDebug()
        {
            CameraManager cameraManager = (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));
            StreamingRules streamingRules = (StreamingRules)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(StreamingRules), GetType());

            if (DebugAllCells)
            {
                Handles.color = Color.blue;

                List<Sector> sectors = StreamingUtility.GetAllScenes("Terrain");

                if (sectors != null)
                {
                    foreach (var sector in sectors)
                    {
                        Handles.DrawWireCube(sector.Bounds.center, sector.Bounds.size);
                    }
                }
            }
            
            if (DebugVisibleCells)
            {
                foreach (VirtualCamera cam in cameraManager.VirtualCameraList)
                {
                    if(cam.Ignored)
                    {
                        continue;
                    }

                    if (RendererStackManager.Instance.EditorPlayModeSimulation || Application.isPlaying)
                    {
                        if (streamingRules.OffsetMaxDistancePreventingUnloadScene != 0)
                        {
                            Handles.color = Color.yellow;
                            DrawHandles.CircleCap(1, cam.Camera.transform.position, Quaternion.LookRotation(Vector3.up), streamingRules.GetMaxLoadingDistance() + streamingRules.OffsetMaxDistancePreventingUnloadScene);
                            
                            //Handles.color = Color.yellow.WithAlpha(0.1f);
                            //Handles.DrawSolidDisc(cam.Camera.transform.position, Vector3.up, streamingRules.GetMaxLoadingDistance() + streamingRules.OffsetMaxDistancePreventingUnloadScene);
                        }
                        
                        if (streamingRules.OffsetMaxLoadingDistanceWithPause != 0)
                        {
                            Handles.color = Color.blue;
                            DrawHandles.CircleCap(1, cam.Camera.transform.position, Quaternion.LookRotation(Vector3.up), streamingRules.GetMaxLoadingDistance());
                            
                            //Handles.color = Color.blue.WithAlpha(0.1f);
                            //Handles.DrawSolidDisc(cam.Camera.transform.position, Vector3.up, streamingRules.GetMaxLoadingDistance());
                        }

                        if (streamingRules.MaxImmediatelyLoadingDistance != 0)
                        {
                            Handles.color = Color.red;
                            DrawHandles.CircleCap(1, cam.Camera.transform.position, Quaternion.LookRotation(Vector3.up), streamingRules.MaxImmediatelyLoadingDistance);
                                
                            //Handles.color = Color.red.WithAlpha(0.1f);
                            //Handles.DrawSolidDisc(cam.Camera.transform.position, Vector3.up, streamingRules.MaxImmediatelyLoadingDistance);
                        }
                    }

                    List<Sector> sectors = FindSector.OverlapSphere(cam.Camera.transform.position, streamingRules.GetMaxLoadingDistance(), GetSectorLayerTag(), false);

                    if (sectors != null)
                    {
                        foreach (var sector in FindSector.OverlapSphere(cam.Camera.transform.position, streamingRules.GetMaxLoadingDistance(), GetSectorLayerTag(), false))
                        {
                            Handles.color = Color.green;
                            Handles.DrawWireCube(sector.Bounds.center, sector.Bounds.size);
                        }
                    }
                }
            }
        }
    }
}
#endif