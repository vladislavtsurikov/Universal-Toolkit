#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor.Integrations.TerrainObjectRenderer;
using VladislavTsurikov.Runtime;
using VladislavTsurikov.Undo.Editor.UndoActions;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnTerrainObject(Group group, RayHit rayHit)
        {
            ExplodePhysicsToolSettings settings = (ExplodePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(ExplodePhysicsTool), typeof(ExplodePhysicsToolSettings));

            int spawnCount = Random.Range(settings.InstancesMin, settings.InstancesMax);

            for (int i = 0; i < spawnCount; i++)
            {
                PrototypeTerrainObject proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                Vector3 centerPosition = rayHit.Point + new Vector3(0, settings.PositionOffsetY, 0);
                Vector3 positionSpawn = centerPosition + Random.insideUnitSphere * (settings.Size / 2);

                if (settings.SpawnFromOnePoint) 
                {
                    positionSpawn = centerPosition;
                }

                InstanceData instanceData = new InstanceData(positionSpawn, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

                PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.SetInstanceData(ref instanceData, 1, rayHit.Normal);

                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

                SimulatedTerrainObjectBody simulatedBody = new SimulatedTerrainObjectBody(proto.RendererPrototype, gameObject);
                PhysicsSimulator.RegisterGameObject(simulatedBody);
                PhysicsSimulator.Activate(DisablePhysicsMode.ObjectTime);

                if (!settings.SpawnFromOnePoint) 
                {
                    Vector3 normal = (instanceData.Position - centerPosition).normalized;
                    PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
                }
                else
                {
                    PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
                }

                //Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));
            }
            
            RandomUtility.ChangeRandomSeed();
        }
        
        public static void SpawnGameObject(Group group, RayHit rayHit)
        {
            ExplodePhysicsToolSettings settings = (ExplodePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(ExplodePhysicsTool), typeof(ExplodePhysicsToolSettings));

            int spawnCount = Random.Range(settings.InstancesMin, settings.InstancesMax);

            for (int i = 0; i < spawnCount; i++)
            {
                PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                Vector3 centerPosition = rayHit.Point + new Vector3(0, settings.PositionOffsetY, 0);
                Vector3 positionSpawn = centerPosition + Random.insideUnitSphere * (settings.Size / 2);

                if (settings.SpawnFromOnePoint) 
                {
                    positionSpawn = centerPosition;
                }

                InstanceData instanceData = new InstanceData(positionSpawn, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

                PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.SetInstanceData(ref instanceData, 1, rayHit.Normal);

                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

                SimulatedBody simulatedBody = new SimulatedBody(gameObject);
                PhysicsSimulator.RegisterGameObject(simulatedBody);
                PhysicsSimulator.Activate(DisablePhysicsMode.ObjectTime);

                if (!settings.SpawnFromOnePoint) 
                {
                    Vector3 normal = (instanceData.Position - centerPosition).normalized;
                    PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, normal * settings.Force);
                }
                else
                {
                    PhysicsUtility.ApplyForce(simulatedBody.Rigidbody, Random.insideUnitSphere.normalized);
                }

                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));
            }
            
            RandomUtility.ChangeRandomSeed();
        }
    }
}
#endif