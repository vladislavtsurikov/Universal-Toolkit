#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.Utility.Runtime;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [MenuItem("Advanced Brush")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail), typeof(PrototypeTerrainTexture)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddToolComponents(new[] { typeof(AdvancedBrushToolSettings), typeof(BrushSettings)})]
    [AddGeneralPrototypeComponents(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)}, new []{typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail), new []{typeof(SpawnDetailSettings), typeof(MaskFilterComponentSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponentSettings)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)}, new []{typeof(ScatterComponentSettings), typeof(FilterSettings)})]
    public class AdvancedBrushTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new SpacingMouseMove();

        private AdvancedBrushToolSettings _advancedBrushToolSettings;
        private BrushSettings _brushSettings;
        
        protected override void OnEnable()
        {
            _advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));
            _brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(BrushSettings));
            
            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }
        
        protected override void DoTool()
        {
            float brushSpacing = _brushSettings.GetCurrentSpacing();
            if(_mouseMove.IsStartDrag)
            {
                if(WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainObject)).Count != 0
                   || WindowData.Instance.SelectedData.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
                {
                    if(brushSpacing < _brushSettings.BrushSize / 2)
                    {
                        brushSpacing = _brushSettings.BrushSize / 2;
                    }
                }
            }
            
            _mouseMove.Spacing = brushSpacing;
            _mouseMove.LookAtSize = _brushSettings.BrushSize;
            
            _mouseMove.Run();
        }

        protected override void HandleKeyboardEvents()
        {
            BrushSettings brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(BrushSettings));

            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings, _mouseMove.Raycast.Point, group);

                if(area.RayHit != null)
                {
                    PaintGroup(group, area);
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint), GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if(rayHit != null)
                {
                    BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings, rayHit.Point, group);

                    if(area.RayHit != null)
                    {
                        PaintGroup(group, area, _advancedBrushToolSettings.EnableFailureRateOnMouseDrag);
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            AdvancedBrushToolVisualisation.Draw(area, WindowData.Instance.SelectionData, GlobalCommonComponentSingleton<LayerSettings>.Instance);
        }

        private void PaintGroup(Group group, BoxArea area, bool dragMouse = false)
        {
            AdvancedBrushToolSettings advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                CoroutineRunner.StartCoroutine(SpawnGroupGameObject(group, area, dragMouse));
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                CoroutineRunner.StartCoroutine(SpawnGroupTerrainObject(group, area, dragMouse));
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                SpawnGroup.SpawnTerrainDetails(group, group.PrototypeList, area);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                SpawnGroup.SpawnTerrainTexture(group, group.GetAllSelectedPrototypes(), area, advancedBrushToolSettings.TextureTargetStrength);
            }
        }

        private IEnumerator SpawnGroupGameObject(Group group, BoxArea area, bool dragMouse)
        {
            AdvancedBrushToolSettings advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));

            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.WaitForNextFrame = false;

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
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

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness);
                    }
                }
            });
        }

        private IEnumerator SpawnGroupTerrainObject(Group group, BoxArea area, bool dragMouse = false)
        {
#if RENDERER_STACK
            AdvancedBrushToolSettings advancedBrushToolSettings = (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolSettings));

            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.WaitForNextFrame = false;

            yield return scatterComponentSettings.Stack.Samples(area, sample =>
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

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnTerrainObject(proto, rayHit, fitness);
                    }
                }
            });
#endif
        }
    }
}
#endif