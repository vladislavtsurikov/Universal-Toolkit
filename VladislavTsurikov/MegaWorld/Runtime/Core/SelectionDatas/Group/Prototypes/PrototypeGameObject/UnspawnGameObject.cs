﻿using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Runtime.Utility;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public static class UnspawnGameObject
    {
        public static void Unspawn(List<Prototype> unspawnPrototypes, bool unspawnSelected)
        {
            List<GameObject> unspawnPrefabs = new List<GameObject>();
            
            foreach (var proto in unspawnPrototypes)
            {
                if(unspawnSelected)
                {
                    if(proto.Selected == false)
                    {
                        continue;
                    }
                }

                unspawnPrefabs.Add((GameObject)proto.PrototypeObject);
            }

            GameObjectUtility.Unspawn(unspawnPrefabs);

#if UNITY_EDITOR
            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
#endif
        }
    }
}