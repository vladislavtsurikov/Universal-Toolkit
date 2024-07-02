using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Name("Layer Settings")]
    public class LayerSettings : Component
    {
        public LayerMask PaintLayers = 1;

        public LayerMask GetCurrentPaintLayers(Type prototypeType)
        {
            if (prototypeType == typeof(PrototypeTerrainDetail) || prototypeType == typeof(PrototypeTerrainTexture))
            {
                if(Terrain.activeTerrain == null)
                {
                    Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                    return PaintLayers;
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