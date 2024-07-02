using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.UnityTerrainTreeConverter
{
    [Name("Unity Terrain Tree Converter")]
    public class UnityTerrainTreeConverter : Extension
    {
        public void ConvertAllUnityTerrainTreeToTerrainObjectRenderer()
        {
            if(Terrain.activeTerrain != null)
            {
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    var terrainData = terrain.terrainData;
                    
                    TreePrototype[] treePrototypeArray = terrainData.treePrototypes;
                    TreeInstance[] treeInstances = terrainData.treeInstances;

                    foreach (var tree in treeInstances)
                    {
                        Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
                        
                        TerrainObjectRendererAPI.AddInstance(treePrototypeArray[tree.prototypeIndex].prefab, GetWorldPosFromTerrain(terrain, tree.position), scale, Quaternion.Euler(0, tree.rotation, 0));
                    }

                    terrain.drawTreesAndFoliage = false;
                }
            }
        }

        public void ConvertUnityTerrainTreeToTerrainObjectRenderer()
        {
            if(Terrain.activeTerrain != null)
            {
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    var terrainData = terrain.terrainData;
                    
                    TreePrototype[] treePrototypeArray = terrainData.treePrototypes;
                    TreeInstance[] treeInstances = terrainData.treeInstances;
                    
                    foreach (var tree in treeInstances)
                    {
                        PrototypeTerrainObject proto = (PrototypeTerrainObject)TerrainObjectRenderer.Instance.SelectionData.GetProto(
                            treePrototypeArray[tree.prototypeIndex].prefab);

                        if(proto != null)
                        {
                            Vector3 scale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
                            
                            TerrainObjectRendererAPI.AddInstance(proto, GetWorldPosFromTerrain(terrain, tree.position), scale, Quaternion.Euler(0, tree.rotation, 0));
                        }
                    }

                    terrain.drawTreesAndFoliage = false;
                }
            }
        }

        public void ConvertTerrainObjectRendererToUnityTerrainTree()
        {
            if(Terrain.activeTerrain != null)
            {
                foreach (Terrain terrain in Terrain.activeTerrains)
                {
                    AddTerrainTreePrototypes(terrain, TerrainObjectRenderer.Instance.SelectionData.PrototypeList);

                    terrain.drawTreesAndFoliage = true;
                }

                UnspawnTerrainTree(TerrainObjectRenderer.Instance.SelectionData.PrototypeList);
                
                List<TerrainObjectCollider> largeObjectInstances = new List<TerrainObjectCollider>();

                foreach (TerrainObjectRendererData item in SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
                {
                    largeObjectInstances.AddRange(item.GetAllInstances());
                }
                
                foreach (var largeObjectCollider in largeObjectInstances)
                {
                    Terrain terrain = GetTerrain(largeObjectCollider.Instance.Position);

                    if(terrain == null)
                    {
                        continue;
                    }

                    TreeInstance treeInstance = new TreeInstance();

                    Vector3 normalizedLocalPos = GetNormalizedLocalPosFromTerrain(terrain, largeObjectCollider.Instance.Position);
                    
                    SetTreeInstanceInfo(ref treeInstance, GetUnityTerrainIndexFromInstantID(terrain, largeObjectCollider.Instance.Proto), normalizedLocalPos, largeObjectCollider.Instance);

                    terrain.AddTreeInstance(treeInstance);
                }
            }
        }

        private void SetTreeInstanceInfo(ref TreeInstance treeInstance, int terrainProtoIdx, Vector3 normalizedLocalPos, TerrainObjectInstance instance)
        {
            treeInstance.prototypeIndex = terrainProtoIdx;
            treeInstance.position = normalizedLocalPos;
            
            float rotationY = instance.Rotation.eulerAngles.y;

            treeInstance.widthScale = instance.Scale.x;
            treeInstance.heightScale = instance.Scale.y;  

            treeInstance.rotation = rotationY * (Mathf.PI / 180f);

            treeInstance.color = Color.white;
            treeInstance.lightmapColor = Color.white;
        }

        private int GetUnityTerrainIndexFromInstantID(Terrain terrain, PrototypeTerrainObject proto)
        {
            if(proto != null)
            {
                for (int treeIndex = 0; treeIndex < terrain.terrainData.treePrototypes.Length; treeIndex++)
                {
                    if (GameObjectUtility.IsSameGameObject(terrain.terrainData.treePrototypes[treeIndex].prefab, proto.Prefab))
                    {
                        return treeIndex;
                    }
                }
            }

            return 0;
        }

        private void AddTerrainTreePrototypes(Terrain terrain, List<Prototype> prototypeList)
        {
            bool found;

            TreePrototype newTree;
            List<TreePrototype> terrainTrees = new List<TreePrototype>(terrain.terrainData.treePrototypes);
            foreach (PrototypeTerrainObject tree in prototypeList)
            {
                found = false;
                foreach (TreePrototype tp in terrainTrees)
                {
                    if (GameObjectUtility.IsSameGameObject(tp.prefab, tree.Prefab))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    newTree = new TreePrototype();
                    newTree.prefab = tree.Prefab;
                    terrainTrees.Add(newTree);
                }
            }
            
            terrain.terrainData.treePrototypes = terrainTrees.ToArray();
        }

        public void RemoveAllPrototypesFromTerrains()
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                List<TreePrototype> terrainTrees = new List<TreePrototype>();

                terrain.terrainData.treePrototypes = terrainTrees.ToArray();

                terrain.Flush();
            }
        }

        public void UnspawnAllTerrainTree()
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                List<TreeInstance> newTrees = new List<TreeInstance>();
                  
                terrain.terrainData.treeInstances = newTrees.ToArray();
            }
        }

        private static void UnspawnTerrainTree(List<Prototype> prototypeList)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                List<TreeInstance> treeInstances = new List<TreeInstance>();
                
                foreach (var treeInstance in terrain.terrainData.treeInstances)
                {
                    bool found = false;
                    foreach (var prototype in prototypeList)
                    {
                        PrototypeTerrainObject prototypeTerrainObject = (PrototypeTerrainObject)prototype;
                            
                        if (GameObjectUtility.IsSameGameObject( terrain.terrainData.treePrototypes[treeInstance.prototypeIndex].prefab, prototypeTerrainObject.Prefab))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        treeInstances.Add(treeInstance);
                    }
                }
                
                terrain.terrainData.treeInstances = treeInstances.ToArray();
            }
        }

        private Vector3 GetWorldPosFromTerrain(Terrain terrain, Vector3 localPosition)
        {
            Vector3 worldPosition = new Vector3(Mathf.Lerp(0f, terrain.terrainData.size.x, localPosition.x),
                Mathf.Lerp(0f, terrain.terrainData.size.y, localPosition.y),
                Mathf.Lerp(0f, terrain.terrainData.size.z, localPosition.z));

            worldPosition += terrain.GetPosition();

            return worldPosition;
        }

        private Vector3 GetNormalizedLocalPosFromTerrain(Terrain terrain, Vector3 worldPosition)
        {
            Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(worldPosition);
            return new Vector3(Mathf.InverseLerp(0f, terrain.terrainData.size.x, terrainLocalPos.x),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.y, terrainLocalPos.y),
                        Mathf.InverseLerp(0f, terrain.terrainData.size.z, terrainLocalPos.z));
        }
        
        public static Terrain GetTerrain(Vector3 location)
        {
            foreach (var terrain in Terrain.activeTerrains)
            {
                var terrainMin = terrain.GetPosition();
                var terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }

            return null;
        }
    }
}