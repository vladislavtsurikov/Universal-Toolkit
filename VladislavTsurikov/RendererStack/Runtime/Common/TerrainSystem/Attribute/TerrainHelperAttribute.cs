using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TerrainHelperAttribute : System.Attribute
    {
        public readonly Type RequiredСomponent;
        public readonly string Name;

        internal TerrainHelperAttribute(Type settingsType, string name)
        {
            RequiredСomponent = settingsType;
            Name = name;
        }

        public List<GameObject> GetTerrains(Scene scene)
        {
            List<UnityEngine.Object> terrains = GameObjectUtility.FindObjectsOfType(RequiredСomponent, scene);
            
            List<GameObject> terrainList = new List<GameObject>();
            
            for (int i = 0; i < terrains.Count; i++)
            {
                Behaviour component = (Behaviour)terrains[i];
                terrainList.Add(component.gameObject);
            }

            return terrainList;
        }
    }
}