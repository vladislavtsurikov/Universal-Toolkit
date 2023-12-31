﻿#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.TerrainSystem
{
    [Serializable]
    public abstract class VirtualTerrain
    {
        [OdinSerialize]
        public Behaviour Target;

        public void InternalInit(Behaviour terrain)
        {
            Target = terrain;
            Init();
        }

        public abstract void Init();
        public abstract Bounds GetTerrainBounds();
        public abstract void RefreshData();
    }
}
#endif