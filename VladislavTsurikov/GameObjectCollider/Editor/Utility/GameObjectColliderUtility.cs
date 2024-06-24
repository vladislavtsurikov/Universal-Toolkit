﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.GameObjectCollider.Editor
{
    public static class GameObjectColliderUtility
    {
        public static List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool checkObbIntersection = false)
        {
            var sceneDataManagers = FindSceneDataManager.OverlapBox(boxCenter, boxSize, boxRotation);

            List<ColliderObject> overlappedObjects = new List<ColliderObject>();

            foreach(var sceneDataManager in sceneDataManagers)
            {
                GameObjectCollider gameObjectCollider = SceneDataStackUtility.InstanceSceneData<GameObjectCollider>(sceneDataManager.Scene);

                if (gameObjectCollider != null)
                {
                    overlappedObjects.AddRange(gameObjectCollider.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool checkObbIntersection = false)
        {
            var sceneDataManagers = FindSceneDataManager.OverlapSphere(sphereCenter, sphereRadius);

            List<ColliderObject> overlappedObjects = new List<ColliderObject>();
            foreach(var sceneDataManager in sceneDataManagers)
            {
                GameObjectCollider gameObjectCollider = SceneDataStackUtility.InstanceSceneData<GameObjectCollider>(sceneDataManager.Scene);

                if (gameObjectCollider != null)
                {
                    overlappedObjects.AddRange(gameObjectCollider.OverlapSphere(sphereCenter, sphereRadius, objectFilter, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static void RemoveNullObjectNodesForAllScenes()
        {
            foreach (var item in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)item.SceneDataStack.GetElement(typeof(GameObjectCollider));

                gameObjectCollider?.RemoveNullObjectNodes();
            }
        }

        public static void HandleTransformChangesForAllScenes()
        {
            foreach (var item in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)item.SceneDataStack.GetElement(typeof(GameObjectCollider));

                gameObjectCollider?.HandleTransformChanges(true);
            }
        }
        
        public static void RemoveNode(GameObject gameObject)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)sceneDataManager.SceneDataStack.GetElement(typeof(GameObjectCollider));

                if (gameObjectCollider == null)
                {
                    continue;
                }
                
                gameObjectCollider.RemoveNode(gameObject);
            }
        }
    }
}
#endif