using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public static class UnspawnGameObject
    {
        public static void Unspawn(List<Prototype> unspawnPrototypes, bool unspawnSelected)
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();

            for (int index = 0; index < allGameObjects.Length; index++)
            {
                List<PrototypeGameObject> prototypes = GetPrototypeUtility.GetPrototypes<PrototypeGameObject>(allGameObjects[index]);

                foreach (var proto in prototypes)
                {
                    if(proto != null && unspawnPrototypes.Contains(proto))
                    {
                        if(unspawnSelected)
                        {
                            if(proto.Selected == false)
                            {
                                continue;
                            }
                        }

                        Object.DestroyImmediate(allGameObjects[index]);
                    }
                }
            }

#if UNITY_EDITOR
            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
#endif
        }
    }
}