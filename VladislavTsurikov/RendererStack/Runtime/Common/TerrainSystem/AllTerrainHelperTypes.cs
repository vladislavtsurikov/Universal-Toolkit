using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem.Attribute;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    public static class AllTerrainHelperTypes 
    {
        public static readonly List<Type> TypeList;

        static AllTerrainHelperTypes()
        {
            TypeList = AllTypesDerivedFrom<TerrainHelper>.TypeList;
        }

        public static bool HasTerrainMonoBehaviourType(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            Behaviour[] list = go.GetComponents<Behaviour>();
            foreach (Behaviour component in list)
            {
                if(component == null)
                {
                    continue;
                }

                foreach (var type in TypeList)
                {
                    Type terrainMonoBehaviourType = type.GetAttribute<TerrainHelperAttribute>().RequiredСomponent;

                    if (component.GetType() == terrainMonoBehaviourType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static TerrainHelper CreateTerrainHelper(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            Behaviour[] list = go.GetComponents<Behaviour>();
            foreach (Behaviour component in list)
            {
                if(component == null)
                {
                    continue;
                }

                foreach (var type in TypeList)
                {
                    Type terrainMonoBehaviourType = type.GetAttribute<TerrainHelperAttribute>().RequiredСomponent;

                    if (component.GetType().Name == terrainMonoBehaviourType.Name)
                    {
                        var terrainType = (TerrainHelper)Activator.CreateInstance(type);
                        terrainType.InternalInit(component);
                        return terrainType;
                    }
                }
            }

            return null;
        }
    }
}