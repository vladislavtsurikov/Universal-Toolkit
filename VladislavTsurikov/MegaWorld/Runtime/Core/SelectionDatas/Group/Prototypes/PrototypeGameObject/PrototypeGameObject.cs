using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Core.Runtime.IconStack.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem.Attributes;
using VladislavTsurikov.Utility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    [Serializable]
    [MenuItem("Unity/GameObject")]
    [AddDefaultGroupComponents(new []{typeof(ContainerForGameObjects)})]
    [DropObjects(new[]{typeof(GameObject)})]
    [MissingIconsWarning("Drag & Drop Prefabs Here")]
    public class PrototypeGameObject : PlacedObjectPrototype
    {
        public override bool IsSamePrototypeObject(Object obj)
        {
            GameObject go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}