#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor.Integrations.TerrainObjectRenderer;
using VladislavTsurikov.Runtime;
using VladislavTsurikov.Undo.Editor.UndoActions;
using VladislavTsurikov.Utility.Runtime;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.Utility
{
    public static class SpawnPrototype 
    {
        public static void SpawnTerrainObject(Group group, PrototypeTerrainObject proto, BoxArea boxArea, RayHit rayHit) 
        {
            float fitness = GrayscaleFromTexture.GetFromWorldPosition(boxArea.Bounds, rayHit.Point, boxArea.Mask);

            if (fitness != 0)
            {
                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    return;
                }
                
                BrushPhysicsToolSettings brushPhysicsToolSettings = (BrushPhysicsToolSettings)ToolsComponentStack.GetElement(typeof(BrushPhysicsTool), typeof(BrushPhysicsToolSettings));
                
                InstanceData instanceData = new InstanceData(rayHit.Point + new Vector3(0, brushPhysicsToolSettings.PositionOffsetY, 0), proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

                PhysicsTransformComponentSettings transformComponentSettings = (PhysicsTransformComponentSettings)proto.GetElement(typeof(PhysicsTransformComponentSettings));
                transformComponentSettings.TransformComponentStack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

                SimulatedTerrainObjectBody simulatedBody = new SimulatedTerrainObjectBody(proto.RendererPrototype, gameObject);
                PhysicsSimulator.RegisterGameObject(simulatedBody);
                brushPhysicsToolSettings.PhysicsEffects.ApplyForce(simulatedBody.Rigidbody);
                PhysicsSimulator.Activate(DisablePhysicsMode.ObjectTime);
                
                RandomUtility.ChangeRandomSeed();
            }
        }
    }
}
#endif