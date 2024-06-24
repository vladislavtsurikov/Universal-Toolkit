#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.TerrainSystem
{
    public static class AllVirtualTerrainTypes 
    {
        private static readonly List<Type> _typeList;

        static AllVirtualTerrainTypes()
        {
            _typeList = AllTypesDerivedFrom<VirtualTerrain>.TypeList;
        }

        public static List<VirtualTerrain> FindAll()
        {
            List<VirtualTerrain> virtualTerrains = new List<VirtualTerrain>();
            
            foreach (Type type in AllTerrainHelperTypes.TypeList)
            {
                TerrainHelperAttribute terrainHelperAttribute = type.GetAttribute<TerrainHelperAttribute>();

                foreach (GameObject go in terrainHelperAttribute.GetTerrains(SceneManager.GetActiveScene()))
                {
                    VirtualTerrain virtualTerrain = CreateTerrainHelper(go);

                    if(virtualTerrain != null)
                    {
                        virtualTerrains.Add(virtualTerrain);
                    }
                }
            }

            return virtualTerrains;
        }

        public static List<VirtualTerrain> FindAll(Type terrainType)
        {
            List<VirtualTerrain> virtualTerrains = new List<VirtualTerrain>();
            
            foreach (Type type in AllTerrainHelperTypes.TypeList)
            {
                TerrainHelperAttribute terrainHelperAttribute = type.GetAttribute<TerrainHelperAttribute>();

                if (terrainType == terrainHelperAttribute.RequiredСomponent)
                {
                    foreach (GameObject go in terrainHelperAttribute.GetTerrains(SceneManager.GetActiveScene()))
                    {
                        VirtualTerrain virtualTerrain = CreateTerrainHelper(go);

                        if(virtualTerrain != null)
                        {
                            virtualTerrains.Add(virtualTerrain);
                        }
                    }
                }
            }

            return virtualTerrains;
        }

        private static VirtualTerrain CreateTerrainHelper(GameObject go)
        {
            if (go == null) return null;

            Behaviour[] list = go.GetComponents<Behaviour>();
            foreach (Behaviour component in list)
            {
                if(component == null)
                {
                    continue;
                }

                foreach (var type in _typeList)
                {
                    Type terrainMonoBehaviourType = type.GetAttribute<TerrainHelperAttribute>().RequiredСomponent;

                    if (component.GetType() == terrainMonoBehaviourType)
                    {
                        var terrainType = (VirtualTerrain)Activator.CreateInstance(type);
                        terrainType.InternalInit(component);
                        return terrainType;
                    }
                }
            }

            return null;
        }
    }
}
#endif