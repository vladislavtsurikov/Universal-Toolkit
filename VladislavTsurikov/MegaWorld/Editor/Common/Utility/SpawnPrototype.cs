#if UNITY_2021_2_OR_NEWER
#else
using UnityEngine.Experimental.TerrainAPI;
#endif
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;
using UnityUtility_Runtime_Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;
#if UNITY_EDITOR
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class SpawnPrototype
    {
        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit, float fitness)
        {
            var overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            var instance = new UnityUtility_Runtime_Instance(rayHit.Point, proto.Prefab.transform.lossyScale,
                proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instance.Position, instance.Scale,
                    instance.Rotation);
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);

#if UNITY_EDITOR
                GameObjectColliderUtility.RegisterGameObjectToCurrentScene(gameObject);
#endif
                gameObject.transform.hasChanged = false;
            }
        }
    }
}
