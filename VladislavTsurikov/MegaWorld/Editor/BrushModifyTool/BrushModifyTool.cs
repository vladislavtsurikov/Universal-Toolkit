#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.GameObjectCollider.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;
using VladislavTsurikov.Utility.Runtime;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool
{
    [MenuItem("Happy Artist/Brush Modify")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    [AddToolComponents(new[] { typeof(BrushSettings), typeof(ModifyTransformSettings)})]
    [AddGroupComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(FilterSettings)})]
    public class BrushModifyTool : ToolWindow
    {
        private Dictionary<GameObject, ModifyInfo> _modifiedGameObjects = new Dictionary<GameObject, ModifyInfo>();
#if RENDERER_STACK
        private Dictionary<TerrainObjectInstance, ModifyInfo> _modifiedTerrainObjects = new Dictionary<TerrainObjectInstance, ModifyInfo>();
#endif
        
        private MouseMove _mouseMove = new MouseMove();
        private BrushSettings _brushSettings;
        private int _updateTicks;
        
        protected override void OnEnable()
        {
            _modifiedGameObjects = new Dictionary<GameObject, ModifyInfo>();
#if RENDERER_STACK
            _modifiedTerrainObjects = new Dictionary<TerrainObjectInstance, ModifyInfo>();
#endif
            
            _brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(BrushSettings));
            
            _mouseMove = new MouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseUp += OnMouseUp;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }
        
        protected override void DoTool()
        {
            _mouseMove.LookAtSize = _brushSettings.BrushSize;
            
            _mouseMove.Run();
        }
        
        protected override void HandleKeyboardEvents()
        {
            BrushSettings brushSettings = (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(BrushSettings));
            
            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            _updateTicks = 0;
        }
        
        private void OnMouseUp()
        {
            _modifiedGameObjects.Clear();
            _modifiedTerrainObjects.Clear();
        }
        
        private void OnMouseDrag(Vector3 dragPoint)
        {
            _updateTicks++;
            
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
                            
                ModifyGroup(group, area);
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            BrushModifyToolVisualisation.Draw(area);
        }

        private void ModifyGroup(Group group, BoxArea area)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                ModifyGameObject(group, area);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                ModifyTerrainObject(group, area);
            }
        }

        private void ModifyTerrainObject(Group group, BoxArea boxArea)
        {
#if RENDERER_STACK
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(BrushModifyTool), typeof(FilterSettings));
            
            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            ModifyTransformSettings modifyTransformSettings = (ModifyTransformSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(ModifyTransformSettings));
            
            PrototypeTerrainObjectOverlap.OverlapBox(boxArea.Bounds, null, false, true, (proto, instance) =>
            {
                if(proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                if (!_modifiedTerrainObjects.TryGetValue(instance, out var modifyInfo))
                {
                    Vector3 randomVector = Random.insideUnitSphere;

                    modifyInfo.RandomScale = 1f - Random.value;
                    modifyInfo.RandomPositionY = 1f - Random.value;
                    modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);
                    
                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new TerrainObjectTransform(instance));
                }

                float fitness = 1;
                Vector3 checkPoint = instance.Position;

                switch (filterSettings.FilterType)
                {
                    case FilterType.SimpleFilter:
                    {
                        if (Physics.Raycast(RayUtility.GetRayDown(checkPoint), out var hit, PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance, 
                                layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
                        {
                            fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                        }
                        break;
                    }
                    case FilterType.MaskFilter:
                    {
                        if(filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                        {
                            fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, checkPoint, filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                        }
                        break;
                    }
                }

                float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, checkPoint, boxArea.Mask);
                    
                fitness *= maskFitness;

                if (modifyInfo.LastUpdate != _updateTicks)
                {
                    modifyInfo.LastUpdate = _updateTicks;

                    Transform transform = new Transform(instance.Position, instance.Scale, instance.Rotation);
        
                    float moveLenght = Event.current.delta.magnitude;
                    
                    modifyTransformSettings.ModifyTransform(ref transform, ref modifyInfo, moveLenght, _mouseMove.StrokeDirection, fitness, Vector3.up);

                    instance.Position = transform.Position;
                    instance.Rotation = transform.Rotation.normalized;
                    instance.Scale = transform.Scale;
                }

                _modifiedTerrainObjects[instance] = modifyInfo;

                return true;
            });
#endif
        }

        private void ModifyGameObject(Group group, BoxArea boxArea)
        {
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(BrushModifyTool), typeof(FilterSettings));
            
            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            ModifyTransformSettings modifyTransformSettings = (ModifyTransformSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(ModifyTransformSettings));

            bool modifyTransform = false;

            PrototypeGameObjectOverlap.OverlapBox(boxArea.Bounds, (proto, go) =>
            {
                if(proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                if (prefabRoot == null)
                {
                    return true;
                }
                    
                if(boxArea.Bounds.Contains(prefabRoot.transform.position) == false)
                {
                    return true;
                }

                if (!_modifiedGameObjects.TryGetValue(prefabRoot, out var modifyInfo))
                {
                    Vector3 randomVector = Random.insideUnitSphere;

                    modifyInfo.RandomScale = 1f - Random.value;
                    modifyInfo.RandomPositionY = 1f - Random.value;

                    modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);

                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new GameObjectTransform(go));
                }

                float fitness = 1;
                Vector3 checkPoint = prefabRoot.transform.position;

                switch (filterSettings.FilterType)
                {
                    case FilterType.SimpleFilter:
                    {
                        if (Physics.Raycast(RayUtility.GetRayDown(checkPoint), out var hit, PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance, 
                                layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
                        {
                            fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                        }
                        break;
                    }
                    case FilterType.MaskFilter:
                    {
                        if(filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                        {
                            fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, prefabRoot.transform.position, filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                        }
                        break;
                    }
                }

                float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, checkPoint, boxArea.Mask);

                fitness *= maskFitness;

                if (modifyInfo.LastUpdate != _updateTicks)
                {
                    modifyInfo.LastUpdate = _updateTicks;

                    Transform transform = new Transform(prefabRoot.transform.position, prefabRoot.transform.localScale, prefabRoot.transform.rotation);

                    float moveLenght = Event.current.delta.magnitude;

                    modifyTransformSettings.ModifyTransform(ref transform, ref modifyInfo, moveLenght, _mouseMove.StrokeDirection, fitness, Vector3.up);
                    
                    prefabRoot.transform.position = transform.Position;
                    prefabRoot.transform.rotation = transform.Rotation;
                    prefabRoot.transform.localScale = transform.Scale;
                    
                    modifyTransform = true;
                }

                _modifiedGameObjects[prefabRoot] = modifyInfo;

                return true;
            });

            if (modifyTransform)
            {
                GameObjectColliderUtility.HandleTransformChangesForAllScenes();
            }
        }
    }
}
#endif