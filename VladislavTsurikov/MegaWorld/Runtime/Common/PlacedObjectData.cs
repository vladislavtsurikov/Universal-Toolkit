using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common
{
    public class PlacedObjectData
    {
        public readonly PlacedObjectPrototype Proto;
        public readonly GameObject GameObject;

        public PlacedObjectData(PlacedObjectPrototype proto, GameObject gameObject)
        {
            Proto = proto; 
            GameObject = gameObject;
        }
    }
}