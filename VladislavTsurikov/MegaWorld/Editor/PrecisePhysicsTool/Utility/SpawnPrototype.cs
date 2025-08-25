#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool
{
    public static class SpawnPrototype
    {
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit)
        {
            var physicsEffects =
                (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            var precisePhysicsToolSettings =
                (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool),
                    typeof(PrecisePhysicsToolSettings));

            var instance = new Instance(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0),
                proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, 1, rayHit.Normal);

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                instance.Position, instance.Scale, instance.Rotation);

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

            physicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }

#if RENDERER_STACK
        public static async UniTask SpawnTerrainObject(Group group, PrototypeTerrainObject proto, RayHit rayHit) 
        {
            ScriptingSystem.SetColliders(new Sphere(rayHit.Point, 500), rayHit);
            
            PhysicsEffects physicsEffects =
 (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            PrecisePhysicsToolSettings precisePhysicsToolSettings =
 (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));
            
            Instance instance =
 new Instance(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TerrainObjectOnDisablePhysics onDisableSimulatedBodyAction =
 new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);
            
            PhysicsTransformComponentSettings transformComponentSettings =
 (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, 1, rayHit.Normal);
            
            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = true;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<ObjectTimeDisablePhysicsMode>();

            TerrainObjectSimulatedBody simulatedBody =
 SimulatedBodyStack.InstantiateSimulatedBody<TerrainObjectSimulatedBody>(proto.Prefab,
                instance.Position, instance.Scale, instance.Rotation, new List<OnDisableSimulatedBodyEvent>{onDisableSimulatedBodyAction});
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
            
            physicsEffects.ApplyForce(simulatedBody.Rigidbody);

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObjectSimulatedBody(simulatedBody));
            
            await UniTask.WaitUntil(() => SimulatedBodyStack.Count == 0);

            ScriptingSystem.RemoveColliders(rayHit);
        }
#endif
    }
}
#endif
