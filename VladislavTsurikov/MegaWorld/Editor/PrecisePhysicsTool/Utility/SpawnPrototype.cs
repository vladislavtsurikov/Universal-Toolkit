#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
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
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit) 
        {
            PhysicsEffects physicsEffects = (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            PrecisePhysicsToolSettings precisePhysicsToolSettings = (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));
            
            Transform transform = new Transform(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, 1, rayHit.Normal);
            
            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation);
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
            
            physicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }
        
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, RayHit rayHit) 
        {
            PhysicsEffects physicsEffects = (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            PrecisePhysicsToolSettings precisePhysicsToolSettings = (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));
            
            Transform transform = new Transform(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TerrainObjectOnDisablePhysics onDisableSimulatedBodyAction = new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);
            
            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, 1, rayHit.Normal);

            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            TerrainObjectSimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation, new List<OnDisableSimulatedBodyAction>{onDisableSimulatedBodyAction});
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
            
            physicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObjectSimulatedBody(simulatedBody));
        }
    }
}
#endif