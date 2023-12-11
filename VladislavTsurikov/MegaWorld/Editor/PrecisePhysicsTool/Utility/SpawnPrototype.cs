#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
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

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePhysicsTool.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit) 
        {
            PhysicsEffects physicsEffects = (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            PrecisePhysicsToolSettings precisePhysicsToolSettings = (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));
            
            InstanceData instanceData = new InstanceData(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.SetInstanceData(ref instanceData, 1, rayHit.Normal);

            GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

            SimulatedBody simulatedBody = new SimulatedBody(gameObject);
            PhysicsSimulator.RegisterGameObject(simulatedBody);
            physicsEffects.ApplyForce(simulatedBody.Rigidbody);
            PhysicsSimulator.Activate(DisablePhysicsMode.ObjectTime);

            group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));
            
            RandomUtility.ChangeRandomSeed();
        }
        
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, RayHit rayHit) 
        {
            PhysicsEffects physicsEffects = (PhysicsEffects)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PhysicsEffects));
            PrecisePhysicsToolSettings precisePhysicsToolSettings = (PrecisePhysicsToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePhysicsTool), typeof(PrecisePhysicsToolSettings));
            
            InstanceData instanceData = new InstanceData(rayHit.Point + new Vector3(0, precisePhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
            transformComponentSettings.TransformComponentStack.SetInstanceData(ref instanceData, 1, rayHit.Normal);

            GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

            SimulatedTerrainObjectBody simulatedBody = new SimulatedTerrainObjectBody(proto.RendererPrototype, gameObject);
            PhysicsSimulator.RegisterGameObject(simulatedBody);
            physicsEffects.ApplyForce(simulatedBody.Rigidbody);
            PhysicsSimulator.Activate(DisablePhysicsMode.ObjectTime);

            RandomUtility.ChangeRandomSeed();
        }
    }
}
#endif