#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    public static class SpawnGroup
    {
        public static IEnumerator SpawnGroupGameObject(Group group, BoxArea area, bool dragMouse)
        {
            AdvancedBrushToolSettings advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));

            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            yield return scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                if(dragMouse)
                {
                    if(Random.Range(0f, 100f) < advancedBrushToolSettings.FailureRateOnMouseDrag)
                    {
                        return;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.RayHit.Point.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, area.Bounds, rayHit);

                    float maskFitness = TextureUtility.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness, true);
                    }
                }
            });
        }

        public static IEnumerator SpawnGroupTerrainObject(Group group, BoxArea area, bool dragMouse = false)
        {
#if RENDERER_STACK
            AdvancedBrushToolSettings advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));

            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            yield return scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                if(dragMouse)
                {
                    if(Random.Range(0f, 100f) < advancedBrushToolSettings.FailureRateOnMouseDrag)
                    {
                        return;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.RayHit.Point.y, sample.y)), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, area.Bounds, rayHit);

                    float maskFitness = TextureUtility.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnTerrainObject(proto, rayHit, fitness, true);
                    }
                }
            });
#else
            yield return null;
#endif
        }
    }
}
#endif