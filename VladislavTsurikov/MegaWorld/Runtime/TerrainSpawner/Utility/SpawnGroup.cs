using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility
{
    public static class SpawnGroup 
    {
        public static IEnumerator SpawnGameObject(Group group, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, bool displayProgressBar)
        {
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            
            scatterComponentSettings.Stack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            
            yield return scatterComponentSettings.Stack.Samples(boxArea, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, boxArea.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);;

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, terrainsMaskManager, rayHit);

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        Common.Utility.Spawn.SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness);
                    }
                }
            });
        }
        
        public static IEnumerator SpawnTerrainObject(Group group, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, bool displayProgressBar)
        {            
#if RENDERER_STACK
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            
            scatterComponentSettings.Stack.SetWaitingNextFrame(displayProgressBar
                ? new DefaultWaitingNextFrame(0.2f)
                : null);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            
            yield return scatterComponentSettings.Stack.Samples(boxArea, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, boxArea.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }
                    
                    float fitness = GetFitness.Get(group, terrainsMaskManager, rayHit);

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        Common.Utility.Spawn.SpawnPrototype.SpawnTerrainObject(proto, rayHit, fitness);
                    }
                }
            });
#endif
        }

        public static IEnumerator SpawnTerrainDetails(Group group, IReadOnlyList<Prototype> protoTerrainDetailList, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea)
        {
            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                yield break;
            }
            
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                if(terrainBounds.Intersects(boxArea.Bounds))
                {
                    if(terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        continue;
                    }
        
                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if(proto.Active == false)
                        {
                            continue;
                        }
                        
                        SpawnPrototype.SpawnTerrainDetails(group, proto, terrainsMaskManager, boxArea, terrain);
                        
                        yield return null;
                    }
                    
                    yield return null;
                }
            }
        }
    }
}