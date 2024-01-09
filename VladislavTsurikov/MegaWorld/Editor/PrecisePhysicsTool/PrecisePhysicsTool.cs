#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ColliderSystem.Runtime.Utility;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
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
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool
{
    [MenuItem("PhysX Painter/Precise Physics")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    [AddGlobalCommonComponents(new []{typeof(LayerSettings)})]
    [AddToolComponents(new []{typeof(PrecisePhysicsToolSettings), typeof(PhysicsEffects)})]
    [AddGeneralPrototypeComponents(new []{typeof(PrototypeGameObject), typeof(PrototypeTerrainObject)}, new []{typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings)})]
    public class PrecisePhysicsTool : ToolWindow
    {
        private SpacingMouseMove _mouseMove = new SpacingMouseMove();

        private PrecisePhysicsToolSettings _precisePhysicsToolSettings;
        
        protected override void OnEnable()
        {
            _precisePhysicsToolSettings = (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }
        
        protected override void DoTool()
        {
            _mouseMove.Spacing = _precisePhysicsToolSettings.Spacing;
            
            _mouseMove.Run();
        }
        
        private void OnMouseDown()
        {
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;
            
            CoroutineRunner.StartCoroutine(PaintGroup(group, _mouseMove.Raycast));
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;
            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            
            RayHit rayHit = ColliderUtility.Raycast(RayUtility.GetRayDown(dragPoint), layerSettings.GetCurrentPaintLayers(group.PrototypeType));

            if(rayHit != null)
            {
                CoroutineRunner.StartCoroutine(PaintGroup(group, rayHit));
            }
        }
        
        private IEnumerator PaintGroup(Group group, RayHit rayHit)
        {
            ScriptingSystem.SetColliders(new Sphere(rayHit.Point, 500), rayHit);
            
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());
                SpawnPrototype.SpawnGameObject(group, proto, rayHit);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());
                SpawnPrototype.SpawnTerrainObject(group, proto, rayHit);
            }
            
            RandomUtility.ChangeRandomSeed();
            
            yield return new YieldCustom(IsDone);
            
            bool IsDone()
            {
                return SimulatedBodyStack.Count == 0;
            }
            
            ScriptingSystem.RemoveColliders(rayHit);
        }

        private void OnRepaint()
        {
            Color color = new Color(0.2f, 0.5f, 0.7f).WithAlpha(0.8f);
            Vector3 position = _mouseMove.Raycast.Point + new Vector3(0, _precisePhysicsToolSettings.PositionOffsetY, 0);
            DrawHandles.HandleButton(0, position, color, 0.7f);
        }
    }
}
#endif