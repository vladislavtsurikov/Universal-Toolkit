using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.Utility
{
    public static class SpawnGroup
    {
        public static IEnumerator SpawnGameObject(Group group, BoxArea area)
        {
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.SetWaitingNextFrame(null);

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnGameObject(group, proto, area, rayHit);
                }
            });
            
            RandomUtility.ChangeRandomSeed();
        }
        
        public static IEnumerator SpawnTerrainObject(Group group, BoxArea area)
        {
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.SetWaitingNextFrame(null);

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.Center.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    SpawnPrototype.SpawnTerrainObject(group, proto, area, rayHit);
                }
            });
            
            RandomUtility.ChangeRandomSeed();
        }
    }
}