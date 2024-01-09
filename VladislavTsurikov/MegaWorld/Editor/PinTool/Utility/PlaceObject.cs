#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool.Utility
{
    public static class PlaceObject 
    {
        public static PlacedObjectData Place(Group group, RayHit rayHit)
        {
            PlacedObjectPrototype proto = GetRandomPrototype.GetRandomSelectedPrototype(group);

            if(proto == null)
            {
                return null;
            }

            Transform transform = new Transform(rayHit.Point, Vector3.one, Quaternion.identity);

            GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, transform.Position, transform.Scale, transform.Rotation);

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
            }

            return new PlacedObjectData(proto, gameObject);
        }
    }
}
#endif