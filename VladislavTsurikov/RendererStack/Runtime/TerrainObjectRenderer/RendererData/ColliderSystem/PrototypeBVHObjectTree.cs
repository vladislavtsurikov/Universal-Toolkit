using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem
{
    public class PrototypeBVHObjectTree
    {
        [NonSerialized]
        private PrototypeTerrainObject _proto;
        
        public readonly BVHObjectTree<TerrainObjectCollider> BVHObjectTree = new BVHObjectTree<TerrainObjectCollider>();

        public int PrototypeID { get; }
        
        public PrototypeTerrainObject Proto
        {
            get
            {
                if(_proto == null)
                {
                    _proto = (PrototypeTerrainObject)TerrainObjectRenderer.Instance.SelectionData.GetProto(PrototypeID);
                }
                return _proto;
            }
        }

        public PrototypeBVHObjectTree(int prototypeID)
        {
            PrototypeID = prototypeID;
        }

        public TerrainObjectCollider RegisterObject(TerrainObjectCollider objectCollider, Cell cell, ColliderCell colliderCell, PrototypeRendererData prototypeStorageRendererData, SceneDataManager sceneDataManager)
        {
            AABB objectAABB = objectCollider.GetAABB();
        
            var treeNode = new BVHNodeAABB<TerrainObjectCollider>(objectCollider);
            treeNode.Position = objectAABB.Center;
            treeNode.Size = objectAABB.Size;
            
            List<object> datas = new List<object>
                { treeNode, sceneDataManager, BVHObjectTree, cell, colliderCell, prototypeStorageRendererData };
            objectCollider.PathToColliderObject = new PathToTerrainObjectCollider(datas);
            objectCollider.Instance.TerrainObjectCollider = objectCollider;

            BVHObjectTree.Tree.InsertLeafNode(treeNode);

            return objectCollider;
        }
        
        public bool Filter(ObjectFilter objectFilter)
        {
            if (Proto == null || !Proto.Active)
            {
                return false;
            }

            if (objectFilter == null)
            {
                return true;
            }
            
            if(!LayerEx.IsLayerBitSet(objectFilter.LayerMask, Proto.Layer))
            {
                return false;
            }

            if(objectFilter.FindOnlySpecificInstancePrefabs)
            {
                if(objectFilter.FindPrefabs.Count == 0 || Proto.Prefab == null)
                {
                    return false;
                }
                
                foreach (GameObject prefab in objectFilter.FindPrefabs)
                {
                    if(GameObjectUtility.IsSameGameObject(prefab, Proto.Prefab))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            
            return true;
        }

        public void ClearBVHObjectTree()
        {
            BVHObjectTree.Clear();
        }
    }
}