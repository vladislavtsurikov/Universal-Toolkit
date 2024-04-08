#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
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
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool
{
    [MenuItem("Spray Brush")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainObject), new []{typeof(SuccessSettings), typeof(OverlapCheckSettings)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SuccessSettings), typeof(OverlapCheckSettings)})]
    [AddPrototypeComponents(typeof(PrototypeTerrainObject), new []{typeof(SimpleTransformComponentSettings)})]
    [AddPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SimpleTransformComponentSettings)})]
    [AddGroupComponents(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)}, new []{typeof(SimpleFilter)})]
    [AddToolComponents(new []{typeof(BrushSettings)})]
    public class SprayBrushTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new SpacingMouseMove();
        
        private BrushSettings _brushSettings;
        
        protected override void OnEnable()
        {
            _brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(SprayBrushTool), typeof(BrushSettings));
            
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
            BrushSettings brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(SprayBrushTool), typeof(BrushSettings)); 
            
            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea boxArea = _brushSettings.GetAreaVariables(_mouseMove.Raycast);

                if(boxArea.RayHit != null)
                {
                    PaintGroup(group, boxArea);
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(dragPoint), 
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if(rayHit != null)
                {
                    BoxArea boxArea = _brushSettings.GetAreaVariables(rayHit);

                    if(boxArea.RayHit != null)
                    {
                        PaintGroup(group, boxArea);
                    }
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea boxArea = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            SprayBrushToolVisualisation.Draw(boxArea);
        }

        private void PaintGroup(Group group, BoxArea boxArea)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                SpawnGroupGameObject(group, boxArea);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                SpawnGroupTerrainObject(group, boxArea);
            }
        }

        private void SpawnGroupTerrainObject(Group group, BoxArea boxArea)
        {
            Vector3 positionForSpawn = boxArea.RayHit.Point + Vector3.ProjectOnPlane(Random.onUnitSphere, boxArea.RayHit.Normal) * boxArea.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), 
                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitness.GetFromSimpleFilter((SimpleFilter)group.GetElement(GetType(), typeof(SimpleFilter)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnTerrainObject(proto, rayHit, fitness);
                }
            }
        }

        private void SpawnGroupGameObject(Group group, BoxArea boxArea)
        {
            Vector3 positionForSpawn = boxArea.RayHit.Point + Vector3.ProjectOnPlane(Random.onUnitSphere, boxArea.RayHit.Normal) * boxArea.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), 
                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitness.GetFromSimpleFilter((SimpleFilter)group.GetElement(GetType(), typeof(SimpleFilter)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnGameObject(group, proto, rayHit, fitness);
                }
            }
        }

        private static void SpawnTerrainObject(PrototypeTerrainObject proto, RayHit rayHit, float fitness)
        {
#if RENDERER_STACK
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            Transform transform = new Transform(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            SimpleTransformComponentSettings transformComponentSettings = (SimpleTransformComponentSettings)proto.GetElement(typeof(SprayBrushTool), typeof(SimpleTransformComponentSettings));
            transformComponentSettings.Stack.ManipulateTransform(ref transform, fitness, rayHit.Normal);

            if(OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, transform))
            {
                TerrainObjectInstance terrainObjectInstance = TerrainObjectRendererAPI.AddInstance(proto.RendererPrototype, transform.Position, transform.Scale, transform.Rotation);
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
            }
#endif
        }

        private static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit, float fitness)
        {
            OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            Transform transform = new Transform(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            SimpleTransformComponentSettings transformComponentSettings = (SimpleTransformComponentSettings)proto.GetElement(typeof(SprayBrushTool), typeof(SimpleTransformComponentSettings));
            transformComponentSettings.Stack.ManipulateTransform(ref transform, fitness, rayHit.Normal);

            if(OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, transform))
            {
                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, transform.Position, transform.Scale, transform.Rotation);
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);

                GameObjectCollider.Runtime.GameObjectCollider.RegisterGameObjectToCurrentScene(gameObject);  
                
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));
                
                gameObject.transform.hasChanged = false;
            }
        }
    }
}
#endif