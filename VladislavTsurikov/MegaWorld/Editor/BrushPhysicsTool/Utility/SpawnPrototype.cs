#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Undo;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Utility.Runtime;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, BoxArea boxArea, RayHit rayHit) 
        {
            float fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness == 0)
            {
                return;
            }
            
            if (Random.Range(0f, 1f) < 1 - fitness)
            {
                return;
            }
                
            BrushPhysicsToolSettings brushPhysicsToolSettings = (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool), typeof(BrushPhysicsToolSettings));
                
            Transform transform = new Transform(rayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, fitness, rayHit.Normal);
                
            TerrainObjectOnDisablePhysics onDisableSimulatedBodyAction = new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);

            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            TerrainObjectSimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation, new List<OnDisableSimulatedBodyAction>{onDisableSimulatedBodyAction});
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
                
            brushPhysicsToolSettings.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);
            
            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObjectSimulatedBody(simulatedBody));
        }
        
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, BoxArea boxArea, RayHit rayHit) 
        {
            float fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness == 0)
            {
                return;
            }
            
            if (Random.Range(0f, 1f) < 1 - fitness)
            {
                return;
            }
                
            BrushPhysicsToolSettings brushPhysicsToolSettings = (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool), typeof(BrushPhysicsToolSettings));
                
            Transform transform = new Transform(rayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, fitness, rayHit.Normal);
                
            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation);
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            brushPhysicsToolSettings.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);
            
            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }
    }
}
#endif