﻿namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class LayerEx
    {
        public static bool IsLayerBitSet(int layerBits, int layerNumber)
        {
            return (layerBits & (1 << layerNumber)) != 0;
        }
    }
}