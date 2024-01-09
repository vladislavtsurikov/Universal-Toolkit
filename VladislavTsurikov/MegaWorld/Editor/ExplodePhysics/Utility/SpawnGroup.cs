using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Core.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics.Utility
{
    public static class SpawnGroup
    {
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

                SpawnPrototype.SpawnGameObject(group, proto, settings, rayHit, positionSpawn, centerPosition);
            }
            
            RandomUtility.ChangeRandomSeed();
        }
        
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

                SpawnPrototype.SpawnTerrainObject(group, proto, settings, rayHit, positionSpawn, centerPosition);
            }
            
            RandomUtility.ChangeRandomSeed();
        }
    }
}