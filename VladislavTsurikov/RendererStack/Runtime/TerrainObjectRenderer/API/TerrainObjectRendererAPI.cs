﻿using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Editor.Utility;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API
{
    public static class TerrainObjectRendererAPI 
    {
        public static void RemoveInstances(int id)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerUtility.GetAllSceneDataManager())
            {
                TerrainObjectRendererData terrainObjectRendererData = (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

                terrainObjectRendererData?.RemoveInstances(id);
            }
        }
        
        public static void RemoveInstances(GameObject prefab)
        {
            RemoveInstances(prefab.GetInstanceID());
        }
        
        public static Prototype AddMissingPrototype(GameObject go)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;

#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif

            Prototype proto = terrainObjectRenderer.SelectionData.GetProto(go);

            if(proto == null)
            {
                proto = terrainObjectRenderer.SelectionData.AddMissingPrototype(go);
            }

            return proto;
        }

        public static void AddMissingPrototype(List<GameObject> gameObjectList)
        {
            foreach (GameObject go in gameObjectList)
            {
                AddMissingPrototype(go);
            }
        }

        public static void AddInstance(GameObject prefab, Vector3 worldPosition, Vector3 scale, Quaternion rotation)
        {
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;
            
#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif
            
            PrototypeTerrainObject proto = (PrototypeTerrainObject)terrainObjectRenderer.SelectionData.AddMissingPrototype(prefab);

            AddInstance(proto, worldPosition, scale, rotation);
        }

        public static void AddInstance(PrototypeTerrainObject proto, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            if(proto == null)
            {
                return;
            }
            
            TerrainObjectRenderer terrainObjectRenderer = TerrainObjectRenderer.Instance;
            
#if UNITY_EDITOR
            if (terrainObjectRenderer == null)
            {
                SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                terrainObjectRenderer = TerrainObjectRenderer.Instance;
            }
#endif

            terrainObjectRenderer.SelectionData.AddMissingPrototype(proto);

            TerrainObjectInstance instance = new TerrainObjectInstance(position, scale, rotation, proto);
            
            TerrainObjectRendererData.AddInstance(instance, Sectorize.Sectorize.GetSectorLayerTag());
        }
        
        public static List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool quadTree = false, bool checkObbIntersection = false)
        {
            var sceneDataManagers = FindSceneDataManager.OverlapBox(boxCenter, boxSize, boxRotation, Sectorize.Sectorize.GetSectorLayerTag());

            List<ColliderObject> overlappedObjects = new List<ColliderObject>();

            foreach(var sceneDataManager in sceneDataManagers)
            {
                if (sceneDataManager == null)
                {
                    continue;
                }

                TerrainObjectRendererData terrainObjectRendererData = (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

                if (terrainObjectRendererData != null)
                {
                    overlappedObjects.AddRange(terrainObjectRendererData.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, terrainObjectRendererData, quadTree, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool quadTree = false, bool checkObbIntersection = false)
        {
            var sectorList = FindSceneDataManager.OverlapSphere(sphereCenter, sphereRadius, Sectorize.Sectorize.GetSectorLayerTag());
            
            List<ColliderObject> overlappedObjects = new List<ColliderObject>();
            foreach(var sceneDataManager in sectorList)
            {
                if (sceneDataManager == null)
                {
                    continue;
                }

                TerrainObjectRendererData terrainObjectRendererData = (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

                if (terrainObjectRendererData != null)
                {
                    overlappedObjects.AddRange(terrainObjectRendererData.OverlapSphere(sphereCenter, sphereRadius, objectFilter, terrainObjectRendererData, quadTree, checkObbIntersection));
                }
            }

            return overlappedObjects;
        }

        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool quadTree, bool checkObbIntersection, Func<TerrainObjectInstance, bool> func)
        {
            foreach (ColliderObject colliderObject in OverlapSphere(sphereCenter, sphereRadius, objectFilter, quadTree, checkObbIntersection))
            {
                if (colliderObject == null)
                {
                    continue;
                }

                TerrainObjectCollider obj = (TerrainObjectCollider)colliderObject;

                func.Invoke(obj.Instance);
            }
        }

        public static void OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool quadTree, bool checkObbIntersection, Func<TerrainObjectInstance, bool> func)
        {
            foreach (var colliderObject in OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, quadTree, checkObbIntersection))
            {
                if (colliderObject == null)
                {
                    continue;
                }
                
                TerrainObjectCollider obj = (TerrainObjectCollider)colliderObject;

                func.Invoke(obj.Instance);
            }
        }
    }
}
