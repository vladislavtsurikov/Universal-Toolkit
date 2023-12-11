#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.Utility;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    [MenuItem("PhysX Painter/Brush Physics")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddToolComponents(new[] { typeof(BrushPhysicsToolSettings), typeof(PhysicsEffects), typeof(BrushSettings) })]
    [AddGeneralPrototypeComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(ScatterComponentSettings)})]
    public class BrushPhysicsTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new SpacingMouseMove();
        
        private BrushSettings _brushSettings;

        protected override void OnEnable()
        {
            _brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool), typeof(BrushSettings));
            
            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }
        
        protected override void DoTool()
        {
            _mouseMove.Spacing = _brushSettings.Spacing;
            _mouseMove.LookAtSize = _brushSettings.BrushSize;
            
            _mouseMove.Run();
        }
        
        protected override void HandleKeyboardEvents()
        {
            _brushSettings.ScrollBrushRadiusEvent();
        }
        
        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings, _mouseMove.Raycast.Point, group);

                if (area.RayHit != null)
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

                if (rayHit != null)
                {
                    BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings, rayHit.Point, group);

                    if (area.RayHit != null)
                    {
                        PaintGroup(group, area);
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            BrushPhysicsToolVisualisation.Draw(area);
        }
        
        private void PaintGroup(Group group, BoxArea area)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());
                //SpawnPrototype.SpawnGameObject(group, proto, rayHit);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                CoroutineRunner.StartCoroutine(SpawnTerrainObject(group, area));
            }
        }
        
        private IEnumerator SpawnTerrainObject(Group group, BoxArea area)
        {
            ScatterComponentSettings scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.Stack.WaitForNextFrame = false;

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
        }
    }
}
#endif