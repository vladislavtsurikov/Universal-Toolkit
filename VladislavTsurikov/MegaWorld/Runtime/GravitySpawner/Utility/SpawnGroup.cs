using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility
{
    public static class SpawnGroup
    {
        public static IEnumerator SpawnGameObject(GravitySpawner gravitySpawner, Group group, TerrainsMaskManager terrainsMaskManager, BoxArea area)
        {
            ScriptingSystem.SetColliders(new Sphere(area.Center, area.Size.x / 2), area);
            
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.SetWaitingNextFrame(new PhysicsWaitingNextFrame(1000));

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                
                if (rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnGameObject(gravitySpawner, group, proto, terrainsMaskManager, area, rayHit);
                }
            });
            
            yield return new YieldCustom(IsDone);
            
            bool IsDone()
            {
#if UNITY_EDITOR
                gravitySpawner.UpdateDisplayProgressBar("Running", "Running " + group.name + " (simulated objects left: " + SimulatedBodyStack.Count + ")");
#endif

                return SimulatedBodyStack.Count == 0;
            }
            
            ScriptingSystem.RemoveColliders(area);
        }
        
        public static IEnumerator SpawnTerrainObject(GravitySpawner gravitySpawner, Group group, TerrainsMaskManager terrainsMaskManager, BoxArea area)
        {
            ScriptingSystem.SetColliders(new Sphere(area.Center, area.Size.x / 2), area);
            
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.SetWaitingNextFrame(new PhysicsWaitingNextFrame(1000));

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }
                    
                    SpawnPrototype.SpawnTerrainObject(gravitySpawner, group, proto, terrainsMaskManager, area, rayHit);
                }
            });
            
            while (!IsDone())
            {
#if UNITY_EDITOR
                gravitySpawner.UpdateDisplayProgressBar("Running", "Running " + group.name + " (simulated objects left: " + SimulatedBodyStack.Count + ")");
#endif

                yield return null;
            }
            
            ScriptingSystem.RemoveColliders(area);
            
            bool IsDone()
            {
                return SimulatedBodyStack.Count == 0;
            }
        }
    }
}