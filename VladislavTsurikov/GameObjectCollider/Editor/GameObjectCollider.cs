﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.GameObjectCollider.Editor
{
    [RequiredSceneData]
    public class GameObjectCollider : RendererSceneData, IRaycast
    {
        [NonSerialized]
        private Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>> _leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
        
        [NonSerialized]
        private BVHObjectTree<BVHGameObject> _sceneObjectTree = new BVHObjectTree<BVHGameObject>();
        
        public delegate void RegisterGameObjectToCurrentSceneDelegate(GameObject gameObject);
        public static RegisterGameObjectToCurrentSceneDelegate RegisterGameObjectToCurrentScene;
        
        protected override void SetupSceneData()
        {
            RefreshObjectTree();
        }

        public override AABB GetAABB()
        {
            return _sceneObjectTree.Tree.GetAABB();
        }

        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter)
        {
            return _sceneObjectTree.RaycastAll(ray, objectFilter);
        }

        public void RefreshObjectTree()
        {
            _sceneObjectTree = new BVHObjectTree<BVHGameObject>();
            _leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            RegisterGameObjects(GameObjectUtility.GetSceneObjects(SceneDataManager.Scene));
        }

        public void RegisterGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject go in gameObjects)
            {
                RegisterGameObject(go);
            }
        }

        public void RegisterGameObjectWithChildren(GameObject gameObject)
        {
            List<GameObject> allChildrenIncludingSelf = gameObject.GetAllChildrenAndSelf();
            
            foreach (GameObject go in allChildrenIncludingSelf)
            {
                RegisterGameObject(go);
            }
        }

        public void RegisterGameObject(GameObject gameObject)
        {
            if (!CanRegisterGameObject(gameObject))
            {
                return;
            }

            GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            BVHGameObject bvhGameObject = new BVHGameObject(gameObject, prefab);

            bvhGameObject.GameObject.transform.hasChanged = false;

            AABB objectAABB = bvhGameObject.GetAABB();
        
            var treeNode = new BVHNodeAABB<BVHGameObject>(bvhGameObject)
            {
                Position = objectAABB.Center,
                Size = objectAABB.Size
            };
            
            _sceneObjectTree.Tree.InsertLeafNode(treeNode);

            if(_leafNodes == null)
            {
                _leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            }

            _leafNodes.Add(bvhGameObject, treeNode); 
            
            SceneObjectsBoundsUtility.ChangeSceneObjectsBounds(SceneDataManager.Sector);
        }

        public void HandleTransformChanges(bool forceCheck = false)
        {
            if(Selection.gameObjects.Length != 0 || forceCheck)
            {
                Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>> changedTransform = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();

                // Loop through all object-to-nodes pairs
                foreach (var pair in _leafNodes)
                {
                    // Can be null if the object was destroyed in the meantime
                    if (pair.Key.GameObject == null)
                    {
                        continue;
                    }

                    if (pair.Key.GameObject.transform.hasChanged)
                    {
                        changedTransform.Add(pair.Key, pair.Value);
                        pair.Key.GameObject.transform.hasChanged = false;
                    }
                }

                foreach (var pair in changedTransform)
                {
                    _sceneObjectTree.Tree.RemoveLeafNode(pair.Value);
                    _leafNodes.Remove(pair.Key);

                    RegisterGameObjectToCurrentScene?.Invoke(pair.Key.GameObject);
                }

                SceneObjectsBoundsUtility.ChangeSceneObjectsBounds(SceneDataManager.Sector);
            }
        }
        
        public void RemoveNode(GameObject gameObject)
        {
            foreach (var item in _leafNodes)
            {
                BVHGameObject bvhGameObject = item.Key;

                if(gameObject.ContainInChildren(bvhGameObject.GameObject))
                {
                    _sceneObjectTree.Tree.RemoveLeafNode(item.Value);
                    _leafNodes.Remove(item.Key);
                    SceneObjectsBoundsUtility.ChangeSceneObjectsBounds(SceneDataManager.Sector);
                    return;
                }
            }
        }

        public void RemoveNullObjectNodes()
        {
            bool foundNull = false;
            var newDictionary = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            foreach(var pair in _leafNodes)
            {
                BVHGameObject bvhGameObject = pair.Value.Data;
                if (bvhGameObject == null || bvhGameObject.GameObject == null)
                {
                    foundNull = true;
                    _sceneObjectTree.Tree.RemoveLeafNode(pair.Value);
                }
                else
                {
                    newDictionary.Add(pair.Key, pair.Value);
                }
            }
            
            if (foundNull)
            {
                _leafNodes.Clear();
                _leafNodes = newDictionary;

                SceneObjectsBoundsUtility.ChangeSceneObjectsBounds(SceneDataManager.Sector);
            }
        }

        private BVHNodeAABB<BVHGameObject> GetLeafNode(GameObject gameObject)
        {
            foreach (var item in _leafNodes)
            {
                if(item.Key.GameObject == gameObject)
                {
                    return item.Value;
                }
            }

            return null;
        }

        private bool CanRegisterGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            if (gameObject.GetComponent<RectTransform>() != null)
            {
                return false;
            } 
            
            if (gameObject.GetComponent<TerrainCollider>() == null)
            {
                if (!gameObject.IsRendererEnabled())
                {
                    return false;
                }
            }

            if(PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
            {
                GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

                LODGroup lodGroup = prefab.GetComponent<LODGroup>();

                if(lodGroup != null)
                {
                    LOD[] lods = lodGroup.GetLODs();

                    if (lods.Length == 0)
                    {
                        return true;
                    }

                    if (lods[0].renderers.Length == 0)
                    {
                        return true;
                    }
                    
                    if (lods[0].renderers[0] == null)
                    {
                        return false;
                    }
                            
                    return lods[0].renderers[0].gameObject == gameObject;
                }
            }

            return true;
        }
        
        public List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool checkObbIntersection = false, List<object> pathDatas = null)
        {
            List<ColliderObject> overlappedObjects = new List<ColliderObject>();
            
            overlappedObjects.AddRange(_sceneObjectTree.OverlapBox(boxCenter,  boxSize, boxRotation, objectFilter, checkObbIntersection, pathDatas));
            
            return overlappedObjects;
        }

        public List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool checkObbIntersection = false, List<object> pathDatas = null)
        {
            List<ColliderObject> overlappedObjects = new List<ColliderObject>();
            
            overlappedObjects.AddRange(_sceneObjectTree.OverlapSphere(sphereCenter,  sphereRadius, objectFilter, checkObbIntersection, pathDatas));
            
            return overlappedObjects;
        }
        
        public void DrawRaycast(Ray ray, Color nodeColor)
        {
            _sceneObjectTree.DrawRaycast(ray, nodeColor);
        }

        public void DrawAllCells(Color nodeColor)
        {
            _sceneObjectTree.DrawAllCells(nodeColor);
        }
    }
}
#endif