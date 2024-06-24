#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.TerrainSystem
{
    [Serializable]
    [TerrainHelper(typeof(Terrain), "Unity terrain")]
    public class UnityTerrain : VirtualTerrain
    {
        [OdinSerialize]  
        private Terrain _terrain;

        public override void Init() 
        {
            _terrain = (Terrain)Target;
        }

        public override Bounds GetTerrainBounds()
        {
            return new Bounds(_terrain.terrainData.bounds.center + _terrain.transform.position, _terrain.terrainData.bounds.size);
        }

        public override void RefreshData()
        {
            
        }
    }
}
#endif