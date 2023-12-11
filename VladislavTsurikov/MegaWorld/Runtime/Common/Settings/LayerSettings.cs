using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [MenuItem("Layer Settings")]
    public class LayerSettings : ComponentStack.Runtime.Component
    {
        public LayerMask PaintLayers = 1;

        public LayerMask GetCurrentPaintLayers(Type prototypeType)
        {
            if (prototypeType == typeof(PrototypeTerrainDetail) || prototypeType == typeof(PrototypeTerrainTexture))
            {
                if(Terrain.activeTerrain == null || Terrain.activeTerrain.gameObject == null)
                {
                    Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                }

                return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
            }
            else
            {
                return PaintLayers;
            }
        }
    }
}