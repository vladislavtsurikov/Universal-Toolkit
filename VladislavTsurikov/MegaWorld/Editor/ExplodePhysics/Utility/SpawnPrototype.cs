#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.PhysicsSimulator.Runtime.Utility;
using VladislavTsurikov.Undo.Editor.UndoActions;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, ExplodePhysicsToolSettings settings, RayHit rayHit, Vector3 positionSpawn, Vector3 centerPosition)
        {
            Transform transform = new Transform(positionSpawn, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, 1, rayHit.Normal);
            
            TerrainObjectOnDisablePhysics onDisableSimulatedBodyAction = new TerrainObjectOnDisablePhysics(group, proto.RendererPrototype);

            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation, new List<OnDisableSimulatedBodyAction>{onDisableSimulatedBodyAction});
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
            
            if (!settings.SpawnFromOnePoint) 
            {
                Vector3 normal = (transform.Position - centerPosition).normalized;
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
            }
            else
            {
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
            }
        }
        
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, ExplodePhysicsToolSettings settings, RayHit rayHit, Vector3 positionSpawn, Vector3 centerPosition)
        {
            Transform transform = new Transform(positionSpawn, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, 1, rayHit.Normal);

            PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

            SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                transform.Position, transform.Scale, transform.Rotation);
                
            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);
            
            if (!settings.SpawnFromOnePoint) 
            {
                Vector3 normal = (transform.Position - centerPosition).normalized;
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
            }
            else
            {
                PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
            }

            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(simulatedBody.GameObject));
        }
    }
}
#endif