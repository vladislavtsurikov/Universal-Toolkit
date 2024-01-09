using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.Utility.Runtime;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnGameObject(GravitySpawner gravitySpawner, Group group, PrototypeGameObject proto, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, RayHit rayHit) 
        {
            float fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness != 0)
            {
                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }
                
                Transform transform = new Transform(rayHit.Point + new Vector3(0, 30, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

                PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, fitness, rayHit.Normal);

                GravitySpawnerGameObject gravitySpawnerGameObject = new GravitySpawnerGameObject(group, terrainsMaskManager, boxArea);
                
                PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

                SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                    transform.Position, transform.Scale, transform.Rotation, new List<OnDisableSimulatedBodyAction>{gravitySpawnerGameObject});
                
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

                gravitySpawner.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

                RandomUtility.ChangeRandomSeed();
            }
        }
        
        public static void SpawnTerrainObject(GravitySpawner gravitySpawner, Group group, PrototypeTerrainObject proto, TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, RayHit rayHit) 
        {
            float fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness != 0)
            {
                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }
                
                Transform transform = new Transform(rayHit.Point + new Vector3(0, 30, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

                PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.ManipulateTransform(ref transform, fitness, rayHit.Normal);

                GravitySpawnerTerrainObject gravitySpawnerTerrainObject = new GravitySpawnerTerrainObject(group, proto.RendererPrototype, terrainsMaskManager, boxArea);
                
                PhysicsSimulator.Runtime.PhysicsSimulator.Activate<ObjectTimeDisablePhysics>();

                SimulatedBody simulatedBody = SimulatedBodyStack.InstantiateSimulatedBody(proto.Prefab,
                        transform.Position, transform.Scale, transform.Rotation, new List<OnDisableSimulatedBodyAction>{gravitySpawnerTerrainObject});
                
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(simulatedBody.GameObject);

                gravitySpawner.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);

                RandomUtility.ChangeRandomSeed();
            }
        }
    }
}